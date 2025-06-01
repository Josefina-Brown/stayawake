using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_Pinball : MonoBehaviour, IGame
{
    [Header("Ball Settings")]
    public Rigidbody2D ballRb;
    public Transform spawnPoint;
    public Collider2D limit;

    [Header("Flippers")]
    public HingeJoint2D leftFlipper;
    public HingeJoint2D rightFlipper;
    public float flipperForce = 1000f;

    [Header("Score Settings")]
    public int score = 0;
    public int targetScore = 100;

    [Header("Lifes")]
    public int maxLives = 3;     // Máximo de vidas
    private int currentLives;    // Vidas actuales

    [Header("Pines")]
    public string pinTag = "Pinball_Pin";
    public int pointsPerPin = 10;

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
        InitializeScreens();
    }

    private void Update()
    {
        if (!isGameStarted) return;

        // Revisar si la bola cayó debajo del límite
        if (ballRb.transform.position.y < limit.transform.position.y)
        {
            LoseLife();
        }

        Flip(leftFlipper, Input.GetKey(KeyCode.A), true);
        Flip(rightFlipper, Input.GetKey(KeyCode.D), false);
    }
    private Vector2 previousDirection = Vector2.zero;

    void FixedUpdate()
    {
        if (!isGameStarted) return;

        Vector2 currentVelocity = ballRb.linearVelocity;
        float speed = currentVelocity.magnitude;

        // Si la velocidad es muy baja, forzamos una corrección para evitar "muertes" de movimiento
        if (speed < 0.5f)
        {
            Vector2 correctedDirection = previousDirection == Vector2.zero ? Vector2.right : previousDirection;
            ballRb.linearVelocity = correctedDirection.normalized * 1f; // Velocidad mínima
            return;
        }

        Vector2 currentDirection = currentVelocity.normalized;

        // Detecta cambio de dirección significativo (dot < 0.98 significa que el ángulo cambió más de ~11 grados)
        float dot = Vector2.Dot(currentDirection, previousDirection);
        if (dot < 0.98f && previousDirection != Vector2.zero)
        {
            PlaySound(0); // Sonido de rebote o cambio de dirección
        }

        // Guardar la dirección para el próximo frame
        previousDirection = currentDirection;
    }

    void Flip(HingeJoint2D flipper, bool isPressed, bool isLeft)
    {
        JointMotor2D motor = flipper.motor;
        if (isLeft)
        {
            motor.motorSpeed = isPressed ? flipperForce : -flipperForce;
        }
        else
        {
            motor.motorSpeed = isPressed ? -flipperForce : flipperForce;
        }
        flipper.motor = motor;
        flipper.useMotor = true;
    }

    public void StartGame()
    {        ticketReward = ticketReward;
        isGameStarted = true;
        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        score = 0;
        currentLives = maxLives;  // Reseteamos las vidas
        ResetBall();
    }

    public void StopGame()
    {
        isGameStarted = false;
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        score = 0;
    }

    public void WinGame()
    {
        PlaySound(3); // Sonido de victoria
        isGameStarted = false;
        winScreen.SetActive(true);
        FindObjectOfType<TicketManager>().currentTickets += ticketReward;
    }

    public void LoseGame()
    {
        PlaySound(4); // Sonido de derrota
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    // Se llama cada vez que la bola cae
    private void LoseLife()
    {
        currentLives--;

        if (currentLives <= 0)
        {
            LoseGame();
        }
        else
        {
            ResetBall();
            Debug.Log("Vidas restantes: " + currentLives);
        }
    }

    void ResetBall()
    {
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballRb.transform.position = spawnPoint.position;
        LaunchBall();
    }

    void LaunchBall()
    {
        PlaySound(2); // Sonido de lanzamiento
        Vector2 launchForce = new Vector2(Random.Range(-1f, 1f), 1).normalized * 1f;
        ballRb.AddForce(launchForce, ForceMode2D.Impulse);
    }

    void InitializeScreens()
    {
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }
}
