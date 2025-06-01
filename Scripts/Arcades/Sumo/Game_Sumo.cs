using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_Sumo : MonoBehaviour, IGame
{
    [Header("Players")]
    public Rigidbody2D playerRb;
    public Rigidbody2D opponentRb;
    public float moveForce = 10f;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform opponentSpawnPoint;

    [Header("Arena Settings")]
    public Transform arenaCenter;
    public float arenaRadius = 5f;
    public int roundsToWin = 1;

    private int playerWins = 0;
    private int opponentWins = 0;
    private bool roundActive = false;

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

    private void Start()
    {
        StopGame();
    }

    public void StartGame()
    {        ticketReward = ticketReward;
        isGameStarted = true;
        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        playerWins = 0;
        opponentWins = 0;

        StartRound();
    }

    public void StopGame()
    {
        isGameStarted = false;
        roundActive = false;

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
                FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador

        PlaySound(2); // Assuming index 1 is a win sound
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        PlaySound(3); // Assuming index 0 is a lose sound
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void StartRound()
    {
        roundActive = true;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        opponentRb.linearVelocity = Vector2.zero;
        opponentRb.angularVelocity = 0f;

        if (playerSpawnPoint != null)
            playerRb.position = playerSpawnPoint.position;

        if (opponentSpawnPoint != null)
            opponentRb.position = opponentSpawnPoint.position;
    }

    void Update()
    {
        if (!isGameStarted || !roundActive) return;

        HandlePlayerInput();
        HandleOpponentAI();
        CheckBoundaries();
    }

    void HandlePlayerInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal")*-1f, Input.GetAxisRaw("Vertical")).normalized;
        playerRb.AddForce(input * moveForce);
    }

    void HandleOpponentAI()
    {
        Vector2 dir = (playerRb.position - opponentRb.position).normalized;
        opponentRb.AddForce(dir * moveForce * 0.5f); // IA más lenta
    }

    void CheckBoundaries()
    {
        float playerDist = Vector2.Distance(playerRb.position, arenaCenter.position);
        float opponentDist = Vector2.Distance(opponentRb.position, arenaCenter.position);

        if (playerDist > arenaRadius)
        {
            roundActive = false;
            opponentWins++;
            Invoke(nameof(NextRound), 1f);
        }
        else if (opponentDist > arenaRadius)
        {
            roundActive = false;
            playerWins++;
            Invoke(nameof(NextRound), 1f);
        }
    }

    void NextRound()
    {
        if (playerWins >= roundsToWin)
        {
            WinGame();
        }
        else if (opponentWins >= roundsToWin)
        {
            LoseGame();
        }
        else
        {
            StartRound();
        }
    }
}
