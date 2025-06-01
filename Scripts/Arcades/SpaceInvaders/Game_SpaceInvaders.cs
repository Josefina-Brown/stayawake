using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
public class Game_SpaceInvaders : MonoBehaviour, IGame
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject bulletPrefab;
    public GameObject enemyBulletPrefab;
    public GameObject shieldBlockPrefab;

    public GameObject player;
    public Transform[] enemySpawns;
    public Transform[] shieldSpawns;
    public Transform playerSpawnPoint;
    public int rows = 3;
    public int columns = 6;
    public float enemyMoveSpeed = 1f;
    public float enemyDropDistance = 0.5f;
    public float enemyFireRate = 2f;

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
                ticketRewardText.text = $"Tickets +{_ticketReward}";
            }
        }
    }

    [SerializeField] private TextMeshPro _ticketRewardText;
    public TextMeshPro ticketRewardText
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

    public List<GameObject> enemies = new();
    private float direction = 1f;
    private float moveTimer = 0f;
    private float moveInterval = 1f;

    private float fireTimer = 0f;

    public void StartGame()
    {
        ticketReward = ticketReward;
        isGameStarted = true;
        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        SpawnEnemies();
        SpawnShields();
    }


    public void StopGame()
    {
        isGameStarted = false;

        foreach (var enemy in enemies) Destroy(enemy);
        enemies.Clear();

        foreach (var shield in GameObject.FindGameObjectsWithTag("Shield_SpaceInvaders"))
        {
            Destroy(shield);
        }

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador

        PlaySound(2);// Sonido de victoria
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {

        PlaySound(3); // Sonido de derrota
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        HandleEnemyMovement();
        HandleEnemyShooting();

    }

    void HandleEnemyMovement()
    {
        moveTimer += Time.deltaTime;

        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;

            bool changeDirection = false;

            var validEnemies = enemies.Where(e => e != null).ToList();
            if (validEnemies.Count == 0) WinGame();

            foreach (var enemy in enemies)
            {
                if (enemy)
                {
                    enemy.transform.position += Vector3.right * direction * enemyMoveSpeed;
                    if (enemy.transform.position.x >= 7f || enemy.transform.position.x <= -7f)
                    {
                        changeDirection = true;
                    }
                }
            }

            if (changeDirection)
            {
                direction *= -1;
                foreach (var enemy in enemies)
                {
                    if (enemy == null) continue;

                    enemy.transform.position += Vector3.down * enemyDropDistance;

                    if (enemy.transform.position.y < -3f)
                    {

                        LoseGame();
                    }
                }
            }
        }
    }

    void HandleEnemyShooting()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= enemyFireRate)
        {
            fireTimer = 0f;

            // Filtrar solo los enemigos válidos
            var validEnemies = enemies.Where(e => e != null).ToList();
            if (validEnemies.Count > 0)
            {
                GameObject shooter = validEnemies[Random.Range(0, validEnemies.Count)];
                Vector3 spawnPos = shooter.transform.position;
                Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
                PlaySound(1); // Sonido de disparo enemigo
            }
        }
    }


    void SpawnEnemies()
    {
        float spacingX = 0.15f;
        float spacingY = 0.2f;

        foreach (Transform basePoint in enemySpawns)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = basePoint.position + new Vector3(col * spacingX, -row * spacingY, 0f);
                    GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                    enemy.GetComponent<Enemy_SpaceInvader>().gameManager = this;
                    enemies.Add(enemy);
                }
            }
        }
    }

    void SpawnShields()
    {
        foreach (var pos in shieldSpawns)
        {
            Instantiate(shieldBlockPrefab, pos.position, Quaternion.identity);
        }
    }

    public void PlayerHit()
    {
        LoseGame();
    }

}


