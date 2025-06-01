using UnityEngine;

public class ArcadeMachine : MonoBehaviour, IInteractable
{
    public enum ArcadeState { Ready, Broken }
    private IGame game;
    public GameObject gameManager;
    public ArcadeState currentState = ArcadeState.Ready;
    public float energyCost = 10f;
    public int coinsCost = 1;
    public Transform minigameCameraPosition;
    public Transform playerController;
    private Vector3 originalPosition;
    private Camera cam;
    private float regularFov;
    public float gameFov = 60.0f;

    public Material readyMaterial;
    public Material brokenMaterial;
    public MeshRenderer screenRenderer;

    [TextArea(3, 6)]
    public string instrucciones;

    public GameManager gameManagerScript;

    private bool instruccionesVisible = false;
    private bool isInteracting = false;  // NUEVO: indica si estás jugando/interactuando

    void Start()
    {
        cam = Camera.main;
        regularFov = cam.fieldOfView;
        game = gameManager.GetComponent<IGame>();
        //gameManagerScript = gameManager.GetComponent<GameManager>();
    }

    void Update()
    {
        if (!isInteracting) return;  // Solo permito toggle instrucciones si está interactuando

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Toggle instrucciones");
            instruccionesVisible = !instruccionesVisible;
            if (gameManagerScript != null)
            {
                gameManagerScript.MostrarOcultarInstrucciones(instrucciones, instruccionesVisible);
            }
        }
    }

    public void Interact()
    {
        PlayerEnergyManager playerEnergy = FindObjectOfType<PlayerEnergyManager>();
        TicketManager ticketManager = FindObjectOfType<TicketManager>();

        if (currentState == ArcadeState.Ready)
        {
            if (playerEnergy.currentEnergy >= energyCost && ticketManager.currentCoins >= coinsCost)
            {
                playerEnergy.currentEnergy -= energyCost;
                ticketManager.SpendCoins(coinsCost);

                Debug.Log("Jugando en la máquina de arcade...");

                originalPosition = playerController.transform.position;
                FindObjectOfType<PlayerController>().FreezePlayer(minigameCameraPosition);
                ActivateMinigame();

                isInteracting = true;  // MARCO que ya está interactuando
                instruccionesVisible = false; // Reinicio el estado para instrucciones al empezar a jugar
                if (gameManagerScript != null)
                    gameManagerScript.MostrarOcultarInstrucciones("", false); // Oculto instrucciones si estaban abiertas
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

        isInteracting = false; // Ya no está interactuando
        instruccionesVisible = false;
        if (gameManagerScript != null)
            gameManagerScript.MostrarOcultarInstrucciones("", false); // Aseguro ocultar instrucciones al salir
    }

    public void ActivateMinigame()
    {
        if (minigameCameraPosition != null)
        {
            Debug.Log("Original position guardada: " + originalPosition);

            game.StartGame();
            cam.fieldOfView = gameFov;
            cam.transform.rotation = minigameCameraPosition.rotation;
            playerController.transform.position = new Vector3(minigameCameraPosition.position.x, minigameCameraPosition.position.y - 0.35f, minigameCameraPosition.position.z);
        }
    }

    public void DeactivateMinigame()
    {
        Debug.Log("Volviendo a posición original: " + originalPosition);
        game.StopGame();
        cam.fieldOfView = regularFov;
        playerController.transform.position = originalPosition;
    }

    public void DamageMachine()
    {
        if (currentState == ArcadeState.Ready)
        {
            currentState = ArcadeState.Broken;
            screenRenderer.material = brokenMaterial;
            Debug.Log("La máquina está rota.");
        }
    }

    public void RepairMachine()
    {
        if (currentState == ArcadeState.Broken)
        {
            currentState = ArcadeState.Ready;
            screenRenderer.material = readyMaterial;
            Debug.Log("La máquina ha sido reparada y está lista para usar.");
        }
    }
}
