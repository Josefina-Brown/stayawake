using UnityEngine;

public class TicketManager : MonoBehaviour
{
    public int currentTickets = 0;
    public int currentCoins = 0; // Variable para almacenar la cantidad de monedas

    public delegate void TicketChanged(int newAmount);

    public event TicketChanged OnTicketChanged;
    public event TicketChanged OnCoinChanged; // Evento para monedas

    public void AddTickets(int amount)
    {
        currentTickets += amount;
        OnTicketChanged?.Invoke(currentTickets);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        OnCoinChanged?.Invoke(currentCoins); // Invoca el evento de monedas
    }

    public bool SpendTickets(int amount)
    {
        if (currentTickets >= amount)
        {
            currentTickets -= amount;
            OnTicketChanged?.Invoke(currentTickets);
            return true;
        }
        return false;
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            OnCoinChanged?.Invoke(currentCoins); // Invoca el evento de monedas
            return true;
        }
        return false;
    }
}
