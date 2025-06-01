using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_Dart : MonoBehaviour, IGame
{
    public Transform crosshair;         // Punto de mira que se mueve
    public Transform targetCenter;      // Centro de la diana
    public float moveSpeed = 3f;        // Velocidad del punto de mira
    public float accuracyRadius = 0.5f; // Radio de acierto para el tiro
    public float gameDuration = 30f;    // Duración total del juego en segundos
    public int targetHits = 5;          // Aciertos necesarios para ganar
    public int maxMisses = 5;           // Fallos máximos permitidos antes de perder

    private float gameTimer;            // Tiempo restante
    private int hits = 0;               // Aciertos acumulados
    private int misses = 0;             // Fallos acumulados

    private Vector3 moveDirection;      // Dirección actual del punto de mira
    private float changeDirectionTimer; // Temporizador para cambiar dirección

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
    {        ticketReward = ticketReward;
        isGameStarted = true;
        gameTimer = gameDuration;
        hits = 0;
        misses = 0;
        changeDirectionTimer = 0f;

        crosshair.position = Vector3.zero;

        moveDirection = GetRandomDirection();

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
        PlaySound(1); // Suena el sonido de victoria
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
        Debug.Log("¡Ganaste!");
    }

    public void LoseGame()
    {
        PlaySound(2); // Suena el sonido de victoria
        isGameStarted = false;
        loseScreen.SetActive(true);
        Debug.Log("Perdiste.");
    }

    void Update()
    {
        MoveCrosshair();

        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0)
        {
            LoseGame();
            return;
        }

        // Mover el punto de mira

        // Detectar disparo con espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowDart();
        }

        // Condiciones de victoria o derrota
        if (hits >= targetHits)
        {
            WinGame();
        }

        if (misses >= maxMisses)
        {
            LoseGame();
        }
    }

    void MoveCrosshair()
    {
        // Cambiar dirección cada cierto tiempo aleatorio
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0f)
        {
            moveDirection = GetRandomDirection();
            changeDirectionTimer = Random.Range(0.1f, 0.5f);
        }

        // Mover la cruz
        crosshair.localPosition += moveDirection * moveSpeed * Time.deltaTime;

        // Limitar movimiento dentro de un rango fijo (ajusta según tu escena)
        crosshair.localPosition = new Vector3(
            Mathf.Clamp(crosshair.localPosition.x, -0.25f, 0.25f),
            Mathf.Clamp(crosshair.localPosition.y, -0.25f, 0.25f),
            0
        );
    }

    void ThrowDart()
    {
        float distance = Vector3.Distance(crosshair.position, targetCenter.position);
        if (distance <= accuracyRadius)
        {
            PlaySound(0); // Suena el sonido de lanzamiento

            hits++;
            Debug.Log("¡Acierto! Hits: " + hits);
        }
        else
        {
            PlaySound(3); // Suena el sonido de fallo
            misses++;
            Debug.Log("Fallaste. Misses: " + misses);
        }
    }

    Vector3 GetRandomDirection()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        return new Vector3(dir2D.x, dir2D.y, 0);
    }
}
