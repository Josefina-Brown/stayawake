using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_CarDodger : MonoBehaviour, IGame
{
    public GameObject playerCarPrefab;
    public GameObject obstacleCarPrefab;
    public Transform[] lanes;
    public float obstacleSpawnRate = 1.5f;
    public float obstacleSpeed = -2f;
    public float gameDuration = 30f;

    private GameObject playerCar;
    private float spawnTimer;
    private float gameTimer;

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
ticketRewardText.text = $"Tickets +{_ticketReward}";
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
    {
        isGameStarted = true;
        gameTimer = 0f;
        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        ticketReward = ticketReward;
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
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (gameTimer >= gameDuration)
        {
            PlaySound(2);
            WinGame();
            return;
        }

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
        GameObject obstacle = Instantiate(obstacleCarPrefab, spawnPos, Quaternion.identity);
        activeObstacles.Add(obstacle);
    }

    void MoveObstacles()
    {
        foreach (var obs in activeObstacles)
        {
            if (obs)
                obs.transform.position += new Vector3(0, 1, 0) * obstacleSpeed * Time.deltaTime;
        }

        activeObstacles.RemoveAll(o => o == null || o.transform.position.z < -10f);
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

        foreach (var obs in activeObstacles)
        {
            if (obs && Vector3.Distance(obs.transform.position, playerCar.transform.position) < 0.1f)
            {
                PlaySound(1);
                LoseGame();
                PlaySound(3);
                return;
            }
        }
    }
}
