using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_Fishing : MonoBehaviour, IGame
{
    public GameObject boatPrefab;
    public GameObject hookPrefab;
    public GameObject fishPrefab;
    public Transform[] fishingZones; // Zone[0] = superficie, Zones[1-3] = bajo el mar

    public float boatSpeed = 3f;
    public float hookSpeed = 2f;
    public float fishSpawnRate = 2f;
    public float fishLifetime = 5f;
    public float gameDuration = 30f;

    private int fishCaughtCount = 0;
    private const int fishToWin = 10;
    private const float timeRewardPerFish = 5f;


    private GameObject boat;
    private GameObject hook;
    private GameObject caughtFish;
    private List<GameObject> fishList = new();

    private float gameTimer;
    private float spawnTimer;
    private float hookDepth = 0f;
    private bool isHookLowering = false;

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
        fishCaughtCount = 0;
        gameTimer = gameDuration;
        isGameStarted = true;
        spawnTimer = 0f;
        hookDepth = 0f;
        isHookLowering = false;
        fishList.Clear();

        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        boat = Instantiate(boatPrefab, fishingZones[0].position, Quaternion.identity);
        hook = Instantiate(hookPrefab, fishingZones[0].position, Quaternion.identity);
    }

    public void StopGame()
    {
        isGameStarted = false;

        if (boat) Destroy(boat);
        if (hook) Destroy(hook);
        if (caughtFish) Destroy(caughtFish);

        foreach (var fish in fishList)
        {
            if (fish) Destroy(fish);
        }
        fishList.Clear();

        noGameScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void WinGame()
    {
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        PlaySound(2); // Play win sound
        isGameStarted = false;
        winScreen.SetActive(true);
    }

    public void LoseGame()
    {
        PlaySound(3); // Play lose sound
        isGameStarted = false;
        loseScreen.SetActive(true);
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0f)
        {
            LoseGame();
            return;
        }

        spawnTimer += Time.deltaTime;


        HandleBoatMovement();
        HandleHookInput();
        MoveHook();
        DetectFish();

        if (spawnTimer >= fishSpawnRate)
        {
            spawnTimer = 0f;
            SpawnFish();
        }

        CleanFishList();
    }

    void HandleBoatMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        boat.transform.position += Vector3.right * -horizontal * boatSpeed * Time.deltaTime;
    }

    void HandleHookInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isHookLowering = true;
        }
        else
        {
            isHookLowering = false;
        }
    }

    void MoveHook()
    {
        if (isHookLowering && hookDepth < 3f)
        {
            hookDepth += Time.deltaTime * hookSpeed;
        }
        else if (!isHookLowering && hookDepth > 0f)
        {
            hookDepth -= Time.deltaTime * hookSpeed;

            if (hookDepth <= 0f && caughtFish != null)
            {
                PlaySound(1); // Play fish caught sound
                Destroy(caughtFish);
                caughtFish = null;
                fishCaughtCount++;
                gameTimer += timeRewardPerFish;

                //Debug.Log("¡Pez atrapado! Total: " + fishCaughtCount + " | Tiempo: " + gameTimer);

                if (fishCaughtCount >= fishToWin)
                {
                    WinGame();
                }
            }

        }

        hookDepth = Mathf.Clamp(hookDepth, 0f, 3f);
        Vector3 boatPos = boat.transform.position;
        hook.transform.position = new Vector3(boatPos.x, fishingZones[0].position.y - hookDepth, boatPos.z);


        if (caughtFish != null)
        {
            caughtFish.transform.position = hook.transform.position;
        }
    }

    void SpawnFish()
    {
        int zoneIndex = Random.Range(1, fishingZones.Length);
        float randomX = Random.Range(-0.2f, 0.2f);
        Vector3 spawnPos = new Vector3(fishingZones[zoneIndex].position.x + randomX, fishingZones[zoneIndex].position.y, fishingZones[zoneIndex].position.z);
        GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
        fishList.Add(fish);
        Destroy(fish, fishLifetime);
    }

    void DetectFish()
    {
        if (caughtFish != null) return;

        foreach (var fish in fishList)
        {
            if (fish != null && Vector3.Distance(fish.transform.position, hook.transform.position) < 0.2f)
            {
                PlaySound(0); // Play fish detected sound
                caughtFish = fish;
                break;
            }
        }
    }

    void CleanFishList()
    {
        fishList.RemoveAll(f => f == null);
    }
}
