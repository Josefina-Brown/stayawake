using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_MotoZoom : MonoBehaviour, IGame
{
    [Header("Player")]
    public Transform player;
    public float laneDistance = 2.5f;
    private int currentLane = 1; // 0 = izquierda, 1 = centro, 2 = derecha

    [Header("Gameplay")]
    public float gameDuration = 30f;
    public int maxLives = 3;
    public int targetSurvivals = 15;

    private float gameTimer;
    private int lives;
    private int successfulDodges = 0;

    [Header("Road Animation")]
    public SpriteRenderer roadRenderer;        // El sprite actual mostrado
    public Sprite roadSpriteA;                // Sprite 1
    public Sprite roadSpriteB;                // Sprite 2
    public float roadFrameRate = 0.2f;        // Velocidad del cambio (en segundos)

    private float roadAnimTimer;
    private bool isRoadA;


    [Header("Enemy Settings")]
    public GameObject[] enemyPrefab;
    public Transform horizonPoint;
    public float spawnInterval = 1.5f;

    private float spawnTimer;

    private List<MotoZoomEnemy> activeEnemies = new List<MotoZoomEnemy>();

    [Header("UI & Interface")]
    [SerializeField] private GameObject _noGameScreen;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private bool _isGameStarted;
    [SerializeField] private int _ticketReward;

    public GameObject noGameScreen { get => _noGameScreen; set => _noGameScreen = value; }
    public GameObject winScreen { get => _winScreen; set => _winScreen = value; }
    public GameObject loseScreen { get => _loseScreen; set => _loseScreen = value; }
    public bool isGameStarted { get => _isGameStarted; set => _isGameStarted = value; }
        public int ticketReward
    {
        get => _ticketReward;
        set
        {
            _ticketReward = value;
            if (ticketRewardText != null)
            {
ticketRewardText.text = $"Tickets +{_ticketReward}";            }
        }
    }

    [SerializeField] private TextMeshPro  _ticketRewardText;
    public TextMeshPro  ticketRewardText
    {
        get => _ticketRewardText;
        set => _ticketRewardText = value;
    }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _gameSounds = new List<AudioClip>();

    public AudioSource audioSource { get => _audioSource; set => _audioSource = value; }
    public List<AudioClip> gameSounds { get => _gameSounds; set => _gameSounds = value; }

    public void PlaySound(int index)
    {
        if (index >= 0 && index < gameSounds.Count && audioSource != null)
        {
            audioSource.PlayOneShot(gameSounds[index]);
        }
    }

    public void StartGame()
    {        ticketReward = ticketReward;
        isGameStarted = true;
        gameTimer = gameDuration;
        lives = maxLives;
        successfulDodges = 0;
        currentLane = 1;
        SetPlayerLane();

        foreach (var enemy in activeEnemies)
            Destroy(enemy?.gameObject);
        activeEnemies.Clear();

        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void StopGame()
    {
        isGameStarted = false;
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
        PlaySound(2); // Play win sound
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        PlaySound(3); // Play lose sound
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        roadAnimTimer += Time.deltaTime;
        if (roadAnimTimer >= roadFrameRate)
        {
            roadAnimTimer = 0f;
            isRoadA = !isRoadA;
            roadRenderer.sprite = isRoadA ? roadSpriteA : roadSpriteB;
        }


        gameTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        HandleInput();
        UpdateEnemies();

        if (lives <= 0)
        {
            LoseGame();
        }

        if (successfulDodges >= targetSurvivals)
        {
            WinGame();
        }

        if (gameTimer <= 0)
        {
            WinGame();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            currentLane--;
            Debug.Log("Current Lane: " + currentLane);
            PlaySound(0); // Play sound for lane change
            SetPlayerLane();
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            PlaySound(0); // Play sound for lane change
            currentLane++;
            Debug.Log("Current Lane: " + currentLane);
            SetPlayerLane();
        }
    }

    void SetPlayerLane()
    {
        Vector3 pos = player.localPosition;
        pos.x = (currentLane - 1) * laneDistance;
        player.localPosition = pos;
    }

    void SpawnEnemy()
    {
        int prefabIndex = Random.Range(0, enemyPrefab.Length);
        GameObject selectedPrefab = enemyPrefab[prefabIndex];

        GameObject enemyGO = Instantiate(selectedPrefab, horizonPoint.position, Quaternion.identity);
        int lane = Random.Range(0, 3);
        Debug.Log("Enemy Lane: " + lane);
        MotoZoomEnemy enemy = enemyGO.AddComponent<MotoZoomEnemy>();
        enemy.Initialize(lane, this, laneDistance);
        activeEnemies.Add(enemy);
    }

    void UpdateEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            enemy.Tick();

            if (enemy.hasFinished)
            {
                if (enemy.lane == currentLane)
                {
                    lives--;
                    Debug.Log("¡Colisión! Vidas restantes: " + lives);
                }
                else
                {
                    successfulDodges++;
                    Debug.Log("Esquivado. Total: " + successfulDodges);
                }

                Destroy(enemy.gameObject);
                activeEnemies.RemoveAt(i);
            }
        }
    }
}
