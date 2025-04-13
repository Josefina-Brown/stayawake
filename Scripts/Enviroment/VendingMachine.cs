using UnityEngine;

[System.Serializable]
public class VendingItem
{
    public string itemName;
    public int ticketCost;
    public float energyRestore;
}


public class VendingMachine : MonoBehaviour, IInteractable
{
    public VendingItem[] items; // Lista de productos disponibles

    public void Interact()
    {
        // Mostrar menú (esto lo vamos a implementar con UI)
        Debug.Log("Tienda abierta. Mostrando menú de productos...");
        VendingUI vendingUI = FindObjectOfType<VendingUI>();
        vendingUI.OpenMenu(this);
        FindObjectOfType<PlayerController>().FreezePlayer();
    }

    public void BuyItem(int index)
    {
        if (index < 0 || index >= items.Length) return;

        VendingItem item = items[index];
        TicketManager ticketManager = FindObjectOfType<TicketManager>();
        PlayerEnergy playerEnergy = FindObjectOfType<PlayerEnergy>();

        if (ticketManager.SpendTickets(item.ticketCost))
        {
            playerEnergy.RestoreEnergy(item.energyRestore);
            FindObjectOfType<PlayerController>().UnfreezePlayer();
            Debug.Log($"Compraste {item.itemName} (+{item.energyRestore} energía).");
        }
        else
        {
            Debug.Log("No tenés suficientes tickets para comprar eso.");
        }
    }

    public void StopInteraction()
    {
       Debug.Log("Tienda cerrada.");
        VendingUI vendingUI = FindObjectOfType<VendingUI>();
       vendingUI.CloseMenu();
       FindObjectOfType<PlayerController>().UnfreezePlayer();
    }
}
