using UnityEngine;

public class ArcadeMachine : MonoBehaviour, IInteractable
{
    public enum ArcadeState
    {
        Ready,
        Broken
    }
    private IGame game; // Referencia al script del minijuego
    public GameObject gameManager; // Referencia al script del minijuego
    public ArcadeState currentState = ArcadeState.Ready; // El estado inicial de la máquina es "Ready"
    public float energyCost = 10f;  // Costo de energía para jugar
    public int coinsCost = 1;    // Costo de monedas para jugar
    public Transform minigameCameraPosition; // Posición donde queremos mover la cámara durante el minijuego
    public Transform playerController; // Referencia a la cámara del jugador
    private Vector3 originalPosition; // Posición original de la cámara
    private Camera cam;
    private float regularFov;
    public float gameFov = 60.0f;

    // Materiales para indicar el estado de la máquina
    public Material readyMaterial;  // Material para cuando está lista
    public Material brokenMaterial; // Material para cuando está rota
    public MeshRenderer screenRenderer; // Para cambiar el material de la máquina

    void Start()
    {
        // Inicializamos la cámara
        cam = Camera.main;
        regularFov = cam.fieldOfView;
        game = gameManager.GetComponent<IGame>(); // Obtenemos el script del minijuego
        // Obtener el componente Renderer para cambiar el material
        Transform modelTransform = transform.Find("Screen");
        // if (modelTransform != null)
        // {
        //     screenRenderer = modelTransform.GetComponent<MeshRenderer>();
        // }
        // else
        // {
        //     Debug.LogWarning("No se encontró un hijo llamado 'Model'.");
        // }

        // // Inicializamos el material de la máquina como listo
        // screenRenderer.material = readyMaterial;
    }


    public void Interact()
    {
        PlayerEnergy playerEnergy = FindObjectOfType<PlayerEnergy>();
        TicketManager ticketManager = FindObjectOfType<TicketManager>();

        // Si la máquina está en estado "Ready" y el jugador tiene suficiente energía
        if (currentState == ArcadeState.Ready)
        {
            if (playerEnergy.currentEnergy >= energyCost && ticketManager.currentCoins >= coinsCost)
            {
                // Gastar energía y monedas
                playerEnergy.currentEnergy -= energyCost; // Gastar energía
                ticketManager.SpendCoins(coinsCost);

                Debug.Log("Jugando en la máquina de arcade...");

                // Congelar al jugador y mover la cámara a la posición del minijuego
            
                FindObjectOfType<PlayerController>().FreezePlayer(minigameCameraPosition);
                ActivateMinigame();  // Activamos el minijuego de ping pong
            }
            else
            {
                Debug.Log("No tenés suficiente energía para jugar.");
            }
        }
        else
        {
            Debug.Log("La máquina está rota. No se puede interactuar.");
        }
    }

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
            game.StartGame(); // Iniciar el minijuego
            cam.fieldOfView = gameFov;
            cam.transform.rotation = minigameCameraPosition.rotation; // Rotar la cámara a la posición del minijuego
            originalPosition = playerController.transform.position;
            playerController.transform.position = new Vector3(minigameCameraPosition.position.x, minigameCameraPosition.position.y-0.35f, minigameCameraPosition.position.z); // Mover la cámara a la posición del minijuego
        }
    }

    public void DeactivateMinigame()
    {
        game.StopGame(); // Detener el minijuego
        cam.fieldOfView = regularFov;
        playerController.transform.position = originalPosition;
    }

    // Método para dañar la máquina y ponerla en estado "Broken"
    public void DamageMachine()
    {
        if (currentState == ArcadeState.Ready)
        {
            currentState = ArcadeState.Broken;
            screenRenderer.material = brokenMaterial;  // Cambiar el material a roto
            Debug.Log("La máquina está rota.");
        }
    }

    // Para restaurar la máquina a su estado inicial
    public void RepairMachine()
    {
        if (currentState == ArcadeState.Broken)
        {
            currentState = ArcadeState.Ready;
            screenRenderer.material = readyMaterial;  // Restaurar el material original
            Debug.Log("La máquina ha sido reparada y está lista para usar.");
        }
    }
}
