using UnityEngine;

public class TicketManager : MonoBehaviour
{
    public int currentTickets = 0;

    public delegate void TicketChanged(int newAmount);
    public event TicketChanged OnTicketChanged;

    public void AddTickets(int amount)
    {
        currentTickets += amount;
        OnTicketChanged?.Invoke(currentTickets);
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
}
