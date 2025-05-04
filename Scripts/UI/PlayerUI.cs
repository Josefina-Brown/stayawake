using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Slider energyBar;
    public TMP_Text moneyText;
    public TMP_Text ticketsText;

    private PlayerEnergy playerEnergy;
    private TicketManager playerWallet;

    void Start()
    {
        playerEnergy = FindObjectOfType<PlayerEnergy>();
        playerWallet = FindObjectOfType<TicketManager>();

        UpdateUI(); // Inicializa con los valores actuales
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerEnergy != null)
        {
            energyBar.maxValue = playerEnergy.maxEnergy;
            energyBar.value = playerEnergy.currentEnergy;
        }

        if (playerWallet != null)
        {
            moneyText.text = "◘ " + playerWallet.currentCoins.ToString("F0");
            ticketsText.text = "◘ " + playerWallet.currentTickets.ToString("F0");
        }
    }
}
