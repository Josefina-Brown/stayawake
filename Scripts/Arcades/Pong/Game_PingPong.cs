using UnityEngine;

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
    public int ticketReward { get => _ticketReward; set => _ticketReward = value; }



    void Start()
    {
        InitializeScreens();
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

        aiTimer += Time.deltaTime;
        if (aiTimer >= aiReactionDelay)
        {
            float targetY = ballRb.position.y + Random.Range(-aiErrorMargin, aiErrorMargin); // Introduce error
            float paddleY = paddleRight.position.y;
            float distanceToMove = targetY - paddleY;

            if (Mathf.Abs(distanceToMove) > 0.03f)
            {
                float moveDistance = Mathf.Sign(distanceToMove) * Mathf.Min(Mathf.Abs(distanceToMove), paddleSpeed * Time.deltaTime);
                paddleRight.position = new Vector3(paddleRight.position.x, paddleRight.position.y + moveDistance, paddleRight.position.z);
            }

            aiTimer = 0f; // Reinicia el timer para el próximo ajuste
        }
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
        float angle = Random.Range(-45f, 45f);  // Ángulo aleatorio para la pelota
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        ballRb.AddForce(direction * ballSpeed, ForceMode2D.Impulse);  // Lanzamos la pelota
    }

    // Método para gestionar la victoria
    public void WinGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    // Método para gestionar la derrota
    public void LoseGame()
    {
        Debug.Log("El juego ha terminado.");
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    // Método para iniciar el juego
    public void StartGame()
    {
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
