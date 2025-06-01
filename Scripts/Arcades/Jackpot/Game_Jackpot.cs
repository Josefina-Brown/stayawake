using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Game_Jackpot : MonoBehaviour, IGame
{
    [Header("Slot Configuration")]
    public Sprite[] slotIcons;                       // Íconos posibles
    public SpriteRenderer[] slotRenderers;           // 3 SpriteRenderers asignados manualmente

    [Header("Game Timing")]
    public float spinInterval = 0.1f;
    public float gameDuration = 15f;

    [Header("UI Screens")]
    [SerializeField] private GameObject _noGameScreen;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _gameSounds = new List<AudioClip>();

    [Header("Rewards")]

    [SerializeField] private int _ticketReward = 0;
    [SerializeField] private int _ticketRewardA = 2;
    [SerializeField] private int _ticketRewardB = 3;
    [SerializeField] private int _ticketRewardC = 4;
    private bool isSpinning = false;
    private float gameTimer = 0f;
    private float[] spinTimers;
    private bool[] isSlotStopped;

    // IGame Implementation
    public GameObject noGameScreen { get => _noGameScreen; set => _noGameScreen = value; }
    public GameObject winScreen { get => _winScreen; set => _winScreen = value; }
    public GameObject loseScreen { get => _loseScreen; set => _loseScreen = value; }
    public bool isGameStarted { get; set; }
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
        isSpinning = true;
        gameTimer = 0f;

        spinTimers = new float[slotRenderers.Length];
        isSlotStopped = new bool[slotRenderers.Length];

        for (int i = 0; i < slotRenderers.Length; i++)
        {
            isSlotStopped[i] = false;
            spinTimers[i] = 0f;
        }

        noGameScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        PlaySound(0); // sonido de inicio
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
        FindObjectOfType<TicketManager>().currentTickets += ticketReward; // Añadir tickets al jugador
        isGameStarted = false;
        winScreen.SetActive(true);
        PlaySound(1); // sonido de victoria
    }

    public void LoseGame()
    {
        isGameStarted = false;
        loseScreen.SetActive(true);
        PlaySound(2); // sonido de perder
    }

    void Update()
    {
        if (!isGameStarted) return;

        gameTimer += Time.deltaTime;

        if (gameTimer >= gameDuration)
        {
            LoseGame();
            return;
        }

        if (isSpinning)
        {
            for (int i = 0; i < slotRenderers.Length; i++)
            {
                if (!isSlotStopped[i])
                {
                    spinTimers[i] += Time.deltaTime;
                    if (spinTimers[i] >= spinInterval)
                    {
                        spinTimers[i] = 0f;
                        int iconIndex = Random.Range(0, slotIcons.Length);
                        slotRenderers[i].sprite = slotIcons[iconIndex];
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopNextSlot();
                PlaySound(3); // sonido de parada
            }

            if (AllSlotsStopped())
            {
                isSpinning = false;
                CheckForPrize();
            }
        }
    }

    void StopNextSlot()
    {
        for (int i = 0; i < isSlotStopped.Length; i++)
        {
            if (!isSlotStopped[i])
            {
                isSlotStopped[i] = true;
                break;
            }
        }
    }

    bool AllSlotsStopped()
    {
        foreach (bool stopped in isSlotStopped)
        {
            if (!stopped) return false;
        }
        return true;
    }

    void CheckForPrize()
    {
        Sprite s0 = slotRenderers[0].sprite;
        Sprite s1 = slotRenderers[1].sprite;
        Sprite s2 = slotRenderers[2].sprite;

        if (s0 == s1 && s1 == s2)
        {
            // Comparar con los íconos para determinar la recompensa
            for (int i = 0; i < slotIcons.Length; i++)
            {
                if (s0 == slotIcons[i])
                {
                    switch (i)
                    {
                        case 0:
                            ticketReward = _ticketRewardA;
                            break;
                        case 1:
                            ticketReward = _ticketRewardB;
                            break;
                        case 2:
                            ticketReward = _ticketRewardC;
                            break;
                        default:
                            ticketReward = 1; // Valor por defecto
                            break;
                    }
                    break;
                }
            }

            WinGame();
        }
        else
        {
            ticketReward = 0; // No hay recompensa
            LoseGame();
        }
    }

}
