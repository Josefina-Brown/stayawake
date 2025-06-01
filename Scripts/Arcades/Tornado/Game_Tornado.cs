using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_Tornado : MonoBehaviour, IGame
{
    [Header("Game Settings")]
    public GameObject[] fallingObjectPrefabs;
    public Transform spawnAreaTopLeft;
    public Transform spawnAreaTopRight;
    public Transform groundLevel;

    public float spawnInterval = 1.5f;
    public float fallSpeed = 0.5f;
    public float horizontalAmplitude = 1f;
    public float horizontalFrequency = 2f;
    public float gameDuration = 30f;

    private float spawnTimer = 0f;
    private float gameTimer;

    private List<FallingObject> fallingObjects = new();

    private int objectsCaught = 0;
    private const int objectsToWin = 15;

    // Nuevo
    [Header("Crosshair Movement Settings")]
    public Transform crosshairTransform;
    public float crosshairSpeed = 5f; // velocidad de movimiento del crosshair con WASD
    public Vector2 crosshairMinBounds; // límite inferior (ej: esquina izquierda abajo en world coords)
    public Vector2 crosshairMaxBounds; // límite superior (ej: esquina derecha arriba en world coords)
    public float detectionRadius = 0.5f;


    [Header("UI & Sounds")]
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
    }

    public void StartGame()
    {        ticketReward = ticketReward;
        isGameStarted = true;
        gameTimer = gameDuration;
        spawnTimer = 0f;
        objectsCaught = 0;

        ClearFallingObjects();

        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void StopGame()
    {
        isGameStarted = false;
        ClearFallingObjects();

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
                FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador

        PlaySound(2); // Sonido victoria
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        PlaySound(3); // Sonido derrota
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;
        // if (gameTimer <= 0f)
        // {
        //     LoseGame();
        //     return;
        // }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnFallingObject();
        }

        UpdateFallingObjects();
        UpdateCrosshairPosition();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckObjectUnderCrosshair();
        }
    }


    // Dentro de UpdateCrosshairPosition:
    void UpdateCrosshairPosition()
    {
        if (crosshairTransform == null) return;

        Vector3 pos = crosshairTransform.localPosition;

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY += 1f;
        if (Input.GetKey(KeyCode.S)) moveY -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;

        Vector3 movement = new Vector3(moveX, moveY, 0f);

        if (movement.magnitude > 1f)
            movement = movement.normalized;

        movement *= crosshairSpeed * Time.deltaTime;

        pos += movement;

        pos.x = Mathf.Clamp(pos.x, crosshairMinBounds.x, crosshairMaxBounds.x);
        pos.y = Mathf.Clamp(pos.y, crosshairMinBounds.y, crosshairMaxBounds.y);

        crosshairTransform.localPosition = pos;
    }




    void SpawnFallingObject()
    {
        float spawnX = Random.Range(spawnAreaTopLeft.position.x, spawnAreaTopRight.position.x);
        Vector3 spawnPos = new Vector3(spawnX, spawnAreaTopLeft.position.y, spawnAreaTopLeft.position.z);
        //GameObject obj = Instantiate(fallingObjectPrefab, spawnPos, Quaternion.identity);
        if (fallingObjectPrefabs.Length == 0) return;

        int index = Random.Range(0, fallingObjectPrefabs.Length);
        GameObject prefab = fallingObjectPrefabs[index];
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        fallingObjects.Add(new FallingObject(obj, fallSpeed, horizontalAmplitude, horizontalFrequency));
    }

    void UpdateFallingObjects()
    {
        for (int i = fallingObjects.Count - 1; i >= 0; i--)
        {
            var fo = fallingObjects[i];
            if (fo.UpdatePosition(Time.deltaTime))
            {
                // toca suelo, pierde vida o termina juego
                fallingObjects[i].Destroy();
                fallingObjects.RemoveAt(i);
                LoseGame();
                break;
            }
        }
    }

    void CheckObjectUnderCrosshair()
    {
        if (crosshairTransform == null) return;

        Vector2 crosshairPos = new Vector2(crosshairTransform.position.x, crosshairTransform.position.y);

        for (int i = fallingObjects.Count - 1; i >= 0; i--)
        {
            if (fallingObjects[i].IsWithinRadius(crosshairPos, detectionRadius))
            {
                PlaySound(0); // sonido captura
                fallingObjects[i].Destroy();
                fallingObjects.RemoveAt(i);
                objectsCaught++;

                if (objectsCaught >= objectsToWin)
                {
                    WinGame();
                }
                break;
            }
        }
    }

    void ClearFallingObjects()
    {
        foreach (var fo in fallingObjects)
        {
            fo.Destroy();
        }
        fallingObjects.Clear();
    }

    void InitializeScreens()
    {
        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    // Clase interna para controlar objetos que caen con movimiento sinusoidal
    private class FallingObject
    {
        public GameObject obj;
        private float fallSpeed;
        private float horizontalAmplitude;
        private float horizontalFrequency;
        private float elapsedTime = 0f;
        private Vector3 startPosition;

        public FallingObject(GameObject obj, float fallSpeed, float horizontalAmplitude, float horizontalFrequency)
        {
            this.obj = obj;
            this.fallSpeed = fallSpeed;
            this.horizontalAmplitude = horizontalAmplitude;
            this.horizontalFrequency = horizontalFrequency;
            this.startPosition = obj.transform.position;
        }

        // Actualiza posición. Retorna true si toca el suelo (y debe ser eliminado)
        public bool UpdatePosition(float deltaTime)
        {
            elapsedTime += deltaTime;

            float xOffset = Mathf.Sin(elapsedTime * horizontalFrequency) * horizontalAmplitude;
            float newY = obj.transform.position.y - fallSpeed * deltaTime;

            obj.transform.position = new Vector3(startPosition.x + xOffset, newY, startPosition.z);

            // Considera que el suelo está en y=0 (o ajustar según necesidad)
            if (obj.transform.position.y <= 0f)
            {
                return true;
            }
            return false;
        }

        // Verifica si se clickeó el objeto (usando un radio simple de detección)
        public bool IsClicked(Vector2 clickPos)
        {
            if (obj == null) return false;
            float radius = 0.5f; // ajustar según tamaño del prefab
            return Vector2.Distance(obj.transform.position, clickPos) <= radius;
        }

        public void Destroy()
        {
            if (obj != null) GameObject.Destroy(obj);
        }
        // Dentro de FallingObject:
        public bool IsWithinRadius(Vector2 position, float radius)
        {
            if (obj == null) return false;
            return Vector2.Distance(obj.transform.position, position) <= radius;
        }
    }



}
