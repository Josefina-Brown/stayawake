using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    public VendingItemData[] items;
    public AudioSource audioSource;
    public void Interact()
    {
        var vendingUI = FindObjectOfType<VendingUI>();
        vendingUI.OpenMenu(items, BuyItem);
        FindObjectOfType<PlayerController>().FreezePlayer();
    }

    public void BuyItem(int index)
    {
        if (index < 0 || index >= items.Length) return;

        var item = items[index];
        var ticketManager = FindObjectOfType<TicketManager>();
        var playerEnergy = FindObjectOfType<PlayerEnergyManager>();

        if (ticketManager.SpendTickets(item.ticketCost))
        {
            audioSource.PlayOneShot(item.soundEffect);

            switch (item.type)
            {
                case VendingItemType.Food:
                    playerEnergy.RestoreEnergy(item.valueRestore);
                    break;

                case VendingItemType.Money:
                    ticketManager.currentCoins += item.valueRestore;
                    break;
            }

            FindObjectOfType<PlayerController>().UnfreezePlayer();
        }
    }


    public void StopInteraction()
    {
        FindObjectOfType<VendingUI>().CloseMenu();
        FindObjectOfType<PlayerController>().UnfreezePlayer();
    }
}
