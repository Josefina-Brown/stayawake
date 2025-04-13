using UnityEngine;

public class ArcadeMachine : MonoBehaviour, IInteractable
{
    public float energyCost = 10f;  // Costo de energía para jugar
    public int ticketReward = 5;     // Tickets que se ganan al jugar

    public Transform minigameCameraPosition; // Posición donde queremos mover la cámara durante el minijuego
    public Transform playerController; // Referencia a la cámara del jugador
    private Vector3 originalPosition; // Posición original de la cámara
    private Camera cam;
    public float regularFov = 20.0f;
    public float gameFov = 60.0f;

    void Start()
    {
        // Inicializamos la cámara
        cam = Camera.main;
    }

    public void Interact()
    {
        PlayerEnergy playerEnergy = FindObjectOfType<PlayerEnergy>();
        TicketManager ticketManager = FindObjectOfType<TicketManager>();

        // Si no estamos jugando, verificamos si el jugador tiene suficiente energía
        if (playerEnergy.currentEnergy >= energyCost)
        {
            FindObjectOfType<PlayerController>().FreezePlayer();
            playerEnergy.RestoreEnergy(-energyCost);  // Restamos la energía para jugar
            ticketManager.AddTickets(ticketReward);   // Añadimos los tickets ganados
            ActivateMinigame();  // Activamos el minijuego de ping pong

            Debug.Log($"Jugaste a la máquina. -{energyCost} energía, +{ticketReward} tickets.");
        }
        else
        {
            Debug.Log("No tenés suficiente energía para jugar.");
        }


    }
    // ✅ Puedes usar esto en otros scripts:


    public void StopInteraction()
    {
        DeactivateMinigame();
        FindObjectOfType<PlayerController>().UnfreezePlayer();
    }

    // Activar el minijuego de ping pong
    public void ActivateMinigame()
    {

        if (minigameCameraPosition != null)
        {
            cam.fieldOfView = gameFov;

            originalPosition = playerController.transform.position;
            playerController.transform.position = minigameCameraPosition.position;

            cam.transform.rotation = minigameCameraPosition.rotation;
        }
    }

    public void DeactivateMinigame()
    {
        cam.fieldOfView = regularFov;

        playerController.transform.position = originalPosition;

    }
}
