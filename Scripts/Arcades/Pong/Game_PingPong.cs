using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_PingPong : MonoBehaviour, IGame
{
    public Rigidbody2D ballRb;
    public Transform spawnPoint;
    public Transform paddleLeft;
    public Transform paddleRight;
    public float paddleSpeed = 5f;
    public float ballSpeed = 0.5f;

    public float aiReactionDelay = 0.5f; // Delay para la IA
    public float aiErrorMargin = 0.1f; // Margen de error para la IA

    private float aiTimer = 0f; // Timer para la IA
    public int scoreToWin = 5;
    private int scoreLeft = 0;

    private float previousBallDirectionX = 0f;
    private int scoreRight = 0;

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



    void Start()
    {
        InitializeScreens();
        previousBallDirectionX = Mathf.Sign(ballRb.linearVelocity.x);

    }
    void FixedUpdate()
    {
        if (!isGameStarted) return;

        if (Mathf.Abs(ballRb.linearVelocity.x) < 0.5f)
        {
            float directionY = Mathf.Sign(ballRb.linearVelocity.y);
            float fixDirectionX = previousBallDirectionX == 0 ? 1 : previousBallDirectionX;

            Vector2 correctedVelocity = new Vector2(fixDirectionX * 0.2f, directionY * ballRb.linearVelocity.magnitude);
            ballRb.linearVelocity = correctedVelocity.normalized * ballRb.linearVelocity.magnitude;
        }

        float currentDirectionX = Mathf.Sign(ballRb.linearVelocity.x);

        if (currentDirectionX != 0 && currentDirectionX != previousBallDirectionX)
        {
            PlaySound(0);
            previousBallDirectionX = currentDirectionX;
        }

    }

    void Update()
    {
        if (isGameStarted)
        {
            MovePaddles();
            CheckBallPosition();
        }

        if (Input.GetKeyDown(KeyCode.R))
            ResetBall();  // Reinicia el juego si presionas la tecla "R"
    }

    // Inicializa las pantallas según el estado
    void InitializeScreens()
    {
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    // Método para mover las palas (paddles)
    // Método para mover las palas (paddles)
    void MovePaddles()
    {
        float moveLeft = Input.GetAxis("Vertical") * paddleSpeed * Time.deltaTime;
        paddleLeft.position = new Vector3(paddleLeft.position.x, paddleLeft.position.y + moveLeft, paddleLeft.position.z);

           
            paddleRight.position = new Vector3(paddleRight.position.x, ballRb.position.y, paddleRight.position.z);


    }


    // Verifica la posición de la pelota y el puntaje
    void CheckBallPosition()
    {
        // Si la pelota se cae fuera de los límites izquierdo o derecho
        if (ballRb.transform.localPosition.x < -0.5f)
        {
            scoreRight++;  // El jugador de la derecha gana un punto
            ResetBall();
        }
        else if (ballRb.transform.localPosition.x > 0.5f)
        {
            scoreLeft++;   // El jugador de la izquierda gana un punto
            ResetBall();
        }

        // Verificar si algún jugador ha ganado
        if (scoreLeft >= scoreToWin)
        {
            WinGame();
        }
        else if (scoreRight >= scoreToWin)
        {
            LoseGame();
        }
    }

    // Método para reiniciar la pelota
    void ResetBall()
    {
        ballRb.linearVelocity = Vector2.zero;   // Detenemos la pelota
        ballRb.transform.position = spawnPoint.position;  // Colocamos la pelota en el punto de inicio
        LaunchBall();   // Relanzamos la pelota
    }

    // Método para lanzar la pelota en una dirección aleatoria
    void LaunchBall()
    {
        float angle = Random.Range(130f, 230f);  // Ángulo aleatorio para la pelota
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        ballRb.AddForce(direction * ballSpeed, ForceMode2D.Impulse);  // Lanzamos la pelota
    }

    // Método para gestionar la victoria
    public void WinGame()
    {
        PlaySound(1);

        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    // Método para gestionar la derrota
    public void LoseGame()
    {
        PlaySound(2);

        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    // Método para iniciar el juego
    public void StartGame()
    {        ticketReward = ticketReward;
        isGameStarted = true;
        scoreLeft = 0;
        scoreRight = 0;
        noGameScreen.SetActive(false);  // Ocultar pantalla de "No Iniciado"
        ResetBall();  // Reiniciar la pelota y comenzar
    }

    // Método para detener el juego
    public void StopGame()
    {
        isGameStarted = false;  // Detener el juego
        noGameScreen.SetActive(true); // Mostrar pantalla de "No Iniciado"
        winScreen.SetActive(false); // Ocultar pantalla de "Ganar"
        loseScreen.SetActive(false); // Ocultar pantalla de "Perder"
    }
}
