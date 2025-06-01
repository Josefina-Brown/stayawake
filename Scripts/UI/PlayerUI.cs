using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Slider energyBar;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ticketsText;

    private PlayerEnergyManager playerEnergy;
    private TicketManager playerWallet;

    void Start()
    {
        playerEnergy = FindObjectOfType<PlayerEnergyManager>();
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
            moneyText.text = playerWallet.currentCoins.ToString();
            ticketsText.text = playerWallet.currentTickets.ToString();
        }
    }
}
