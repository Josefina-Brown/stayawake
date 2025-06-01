using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_Sky : MonoBehaviour, IGame
{
    public GameObject playerCarPrefab;
    public GameObject[] obstacleCarPrefab;
    public Transform[] lanes;
    public float obstacleSpawnRate = 1.5f;
    public float obstacleSpeed = -2f;
    private float gameDuration = 5f;

    private GameObject playerCar;
    private float spawnTimer;
    public float gameTimer;

    private int caughtCount = 0;
    private int missedCount = 0;


    private List<GameObject> activeObstacles = new();

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
    caughtCount = 0;
    missedCount = 0;
    spawnTimer = 0f;

    noGameScreen.SetActive(false);
    winScreen.SetActive(false);
    loseScreen.SetActive(false);

    Vector3 startPos = lanes[1].position + new Vector3(0, -1, 0) * 1f;
    playerCar = Instantiate(playerCarPrefab, startPos, Quaternion.identity);
}


    public void StopGame()
    {
        isGameStarted = false;
        if (playerCar) Destroy(playerCar);
        foreach (var obs in activeObstacles) Destroy(obs);
        activeObstacles.Clear();

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        PlaySound(2);
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador

        PlaySound(3);
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // if (gameTimer >= gameDuration)
        // {
        //     ticketReward = Mathf.Max(0, (int)(gameDuration));
        //     LoseGame();
        //     return;
        // }

        if (spawnTimer >= obstacleSpawnRate)
        {
            spawnTimer = 0f;
            SpawnObstacle();
        }

        MoveObstacles();
        HandlePlayerInput();
        CheckCollisions();
    }

    void SpawnObstacle()
    {

        int laneIndex = Random.Range(0, lanes.Length);
        Vector3 spawnPos = lanes[laneIndex].position;

        int prefabIndex = Random.Range(0, obstacleCarPrefab.Length);
        GameObject selectedPrefab = obstacleCarPrefab[prefabIndex];

        GameObject obstacle = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        activeObstacles.Add(obstacle);
    }

    void MoveObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obs = activeObstacles[i];
            if (!obs) continue;

            obs.transform.position += new Vector3(0, 1, 0) * obstacleSpeed * Time.deltaTime;

            if (obs.transform.position.y < -5f) // Si se pasó de largo
            {
                missedCount++; // Fallado
                Destroy(obs);
                activeObstacles.RemoveAt(i);

                if (missedCount >= 3)
                {
                    EndGame();
                    break;
                }
            }
        }
    }


    void HandlePlayerInput()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySound(0);
            MovePlayer(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(0);
            MovePlayer(1);
        }
    }

    void MovePlayer(int direction)
    {
        int currentLane = GetCurrentLaneIndex(playerCar.transform.position);
        int newLane = Mathf.Clamp(currentLane + direction, 0, lanes.Length - 1);
        Vector3 newPos = new Vector3(lanes[newLane].position.x, playerCar.transform.position.y, playerCar.transform.position.z);
        playerCar.transform.position = newPos;
    }

    int GetCurrentLaneIndex(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int index = 0;

        for (int i = 0; i < lanes.Length; i++)
        {
            float dist = Mathf.Abs(position.x - lanes[i].position.x);
            if (dist < minDistance)
            {
                minDistance = dist;
                index = i;
            }
        }

        return index;
    }

    void CheckCollisions()
    {
        if (playerCar == null) return;

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obs = activeObstacles[i];
            if (obs && Vector3.Distance(obs.transform.position, playerCar.transform.position) < 0.1f)
            {
                PlaySound(1);
                caughtCount++; // Atrapado
                Destroy(obs);
                activeObstacles.RemoveAt(i);
                break;
            }
        }
    }

    void EndGame()
{
    isGameStarted = false;

    if (playerCar) Destroy(playerCar);
    foreach (var obs in activeObstacles) Destroy(obs);
    activeObstacles.Clear();

    if (caughtCount >= 20)
    {
        ticketReward = caughtCount - 20;
        WinGame();
    }
    else
    {
        ticketReward = 0;
        LoseGame();
    }
}



}
