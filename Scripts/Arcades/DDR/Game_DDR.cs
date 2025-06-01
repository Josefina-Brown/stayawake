using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_DDR : MonoBehaviour, IGame
{
    public GameObject[] arrowPrefab;
    public Transform[] arrowSpawnPoints;
    public Transform targetZone;
    public float arrowSpeed = 2f;
    public float spawnInterval = 1f;
    public float gameDuration = 30f;
    public int targetScore = 10;
    public int maxMisses = 5;

    private float spawnTimer;
    private float gameTimer;
    private bool isSpawning = false;

    private List<GameObject> arrows = new();
    private KeyCode[] arrowKeys = { KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.RightArrow };

    private Queue<KeyCode> keyQueue = new();

    private int score = 0;
    private int misses = 0;
    [SerializeField] private AudioClip backgroundMusic;

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
            {ticketRewardText.text = $"Tickets +{_ticketReward}";
            }
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
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }


        isGameStarted = true;
        gameTimer = gameDuration;
        spawnTimer = 0f;
        score = 0;
        misses = 0;
        isSpawning = true;

        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        ClearArrows();
    }

    public void StopGame()
    {
        if (audioSource != null) audioSource.Stop();


        isGameStarted = false;
        isSpawning = false;
        ClearArrows();

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
        PlaySound(2); // Play win sound
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        isSpawning = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        PlaySound(3); // Play lose sound
        isGameStarted = false;
        isSpawning = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0)
        {
            //LoseGame();
            //return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnArrow();
        }

        MoveArrows();
        HandleInput();

        if (score >= targetScore)
        {
            WinGame();
        }

        if (misses >= maxMisses)
        {
            LoseGame();
        }
    }

    void SpawnArrow()
    {
        int index = Random.Range(0, 4);
        GameObject arrow = Instantiate(arrowPrefab[index], arrowSpawnPoints[index].position, Quaternion.identity);

        arrow.GetComponent<Arrow>().Init(arrowKeys[index], targetZone.position.y, arrowSpeed);
        arrows.Add(arrow);
    }


    void MoveArrows()
    {
        foreach (var arrow in arrows)
        {
            if (arrow != null)
            {
                arrow.transform.Translate(Vector3.up * arrowSpeed * Time.deltaTime);
            }
        }

        arrows.RemoveAll(a => a == null);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) CheckHit(KeyCode.LeftArrow);
        if (Input.GetKeyDown(KeyCode.DownArrow)) CheckHit(KeyCode.DownArrow);
        if (Input.GetKeyDown(KeyCode.UpArrow)) CheckHit(KeyCode.UpArrow);
        if (Input.GetKeyDown(KeyCode.RightArrow)) CheckHit(KeyCode.RightArrow);
    }

    void CheckHit(KeyCode key)
    {
        foreach (var arrow in arrows)
        {
            if (arrow == null) continue;

            Arrow arrowScript = arrow.GetComponent<Arrow>();
            if (arrowScript.arrowKey == key && Mathf.Abs(arrow.transform.position.y - targetZone.position.y) < 0.5f)
            {
                PlaySound(0);

                Destroy(arrow);
                score++;
                Debug.Log("Acierto! Score: " + score);
                return;
            }
        }
        PlaySound(1); // Play miss sound
        misses++;
        Debug.Log("Fallo! Misses: " + misses);
    }

    void ClearArrows()
    {
        foreach (var arrow in arrows)
        {
            if (arrow != null)
            {
                Destroy(arrow);
            }
        }
        arrows.Clear();
    }
}
