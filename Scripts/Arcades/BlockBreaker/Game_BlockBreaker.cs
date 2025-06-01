using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_BlockBreaker : MonoBehaviour, IGame
{
    public Rigidbody2D ballRb;
    public Transform spawnPoint;
    public Collider2D[] walls;
    public Collider2D limit; // Límite donde el jugador pierde  
    public int blocksDestroyed = 0; // Contador de bloques destruidos
    public GameObject[] blocks; // Array para almacenar todos los bloques

    public Transform bumperTransform;  // Referencia al bumper
    public float bumperSpeed = 5f;    // Velocidad del bumper
    private float bumperInitialX;
    public float bumperLeftLimit;   // Límite izquierdo del bumper
    public float bumperRightLimit;  // Límite derecho del bumper
    private Vector2 ballRblastVelocity;

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


    void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag("BlockBreaker_Block"); // Encontrar todos los bloques en la escena
        InitializeScreens();
        bumperInitialX = bumperTransform.localPosition.x;
        ticketReward = ticketReward;
    }

    void Update()
    {
        if (!isGameStarted)
        {
            ResetBall(); // Reiniciar la pelota si el juego no ha comenzado
        }

        // Si se presiona la tecla R, reiniciamos la pelota
        if (Input.GetKeyDown(KeyCode.R))
            ResetBall();

        // Si la pelota toca el límite, se pierde el juego
        if (ballRb.transform.position.y < limit.transform.position.y)
        {
            LoseGame();
        }

        // Mover el bumper de izquierda a derecha con las teclas
        MoveBumper();
    }

    private void FixedUpdate()
    {
        // ballRblastVelocity = ballRb.linearVelocity; // Guardamos la velocidad de la pelota
    }

    // Inicializa las pantallas según el estado
    void InitializeScreens()
    {
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    // Método para resetear la pelota
    void ResetBall()
    {
        ballRb.linearVelocity = Vector2.zero; // Reiniciar la velocidad de la pelota
        ballRb.angularVelocity = 0f;    // Reiniciar la rotación
        ballRb.transform.position = spawnPoint.position; // Posicionar la pelota en el spawn
        LaunchBall(); // Lanzar la pelota
    }

    // Método para lanzar la pelota en un ángulo determinado
    void LaunchBall()
    {
        float angle = 30f; // Ángulo en el que se lanza la pelota
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        ballRb.AddForce(direction * 0.7f, ForceMode2D.Impulse); // Lanzar la pelota con una fuerza inicial
    }

    // Método para destruir un bloque y aumentar el contador
    public void GetPoint()
    {
        blocksDestroyed++;
        PlaySound(0);
        if (blocksDestroyed >= blocks.Length)
        {
            WinGame();
        }
    }
    public void GetHit()
    {
        PlaySound(1);

    }

    // Método para mover el bumper de izquierda a derecha
    void MoveBumper()
    {
        if (isGameStarted)
        {
            float moveInput = Input.GetAxis("Horizontal"); // Obtiene la entrada de las teclas de dirección

            // Calculamos la nueva posición LOCAL del bumper
            float newX = bumperTransform.localPosition.x + moveInput * bumperSpeed * Time.deltaTime;

            // Limitamos dentro de los bordes locales
            if (newX < bumperLeftLimit)
                newX = bumperLeftLimit;
            else if (newX > bumperRightLimit)
                newX = bumperRightLimit;

            // Aplicamos la nueva posición LOCAL
            bumperTransform.localPosition = new Vector3(newX, bumperTransform.localPosition.y, bumperTransform.localPosition.z);
        }
    }

    // Método para iniciar el juego
    public void StartGame()
    {
        foreach (var block in blocks)
        {
            block.SetActive(true); // Activar todos los bloques
        }
        isGameStarted = true;
        noGameScreen.SetActive(false); // Ocultar la pantalla de inicio
        ResetBall(); // Resetea la pelota y comienza
    }

    // Método para detener el juego
    public void StopGame()
    {
        isGameStarted = false; // Detener el juego

        noGameScreen.SetActive(true); // Mostrar pantalla de "No Iniciado"
        winScreen.SetActive(false); // Ocultar pantalla de "Ganar"
        loseScreen.SetActive(false); // Ocultar pantalla de "Perder"

        blocksDestroyed = 0; // Reiniciar el contador de bloques destruidos
    }

    // Método para ganar el juego
    public void WinGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
        PlaySound(2); // Sonido de victoria
    }


    // Método para perder el juego
    public void LoseGame()
    {
        isGameStarted = false; // Detener el juego
        loseScreen.SetActive(true); // Mostrar pantalla de "Perder"
        PlaySound(3);// Sonido de derrota
    }

}
