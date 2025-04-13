using UnityEngine;

public class MiniGravityGame : MonoBehaviour

{
    public Rigidbody2D ball;               // Referencia al Rigidbody2D de la pelota
    public GameObject ground;              // El suelo (Sprite)
    public float moveSpeed = 5f;           // Velocidad de movimiento horizontal
    public float gravityStrength = 9.8f;   // Fuerza de la gravedad

    public Transform gravityReference;     // El objeto que contiene la orientación del minijuego (normalmente el arcade)

    private bool gameActive = false;       // Estado del juego

    void Start()
    {
        ball.gravityScale = 0;  // Desactivamos la gravedad global de Unity
        ground.SetActive(true); // Aseguramos que el suelo esté activado al inicio

        // Alineamos el suelo al eje local
        AlignWithGravityReference(ground);

        ActivateGame();         // O espera que se active externamente
    }

    void Update()
    {
        if (!gameActive) return;

        // Movimiento horizontal: calculamos la dirección local en relación con el 'gravityReference'
        Vector2 localRight = gravityReference.right; // El eje 'derecho' local
        float moveInput = Input.GetAxis("Horizontal"); // Obtener la entrada horizontal (A/D o flechas)

        // Aplicar la fuerza horizontal en función de la entrada del jugador
        ball.AddForce(localRight * moveInput * moveSpeed);
    }

    void FixedUpdate()
    {
        if (!gameActive) return;

        // Calculamos la gravedad en función de la rotación local (el "abajo" de la pantalla del arcade)
        Vector2 localDown = -gravityReference.up;  // El eje 'abajo' local, usando la rotación del arcade

        // Aplicar la gravedad local
        ball.AddForce(localDown * gravityStrength);
    }

    // Activar el minijuego y reiniciar el estado de la bola
    public void ActivateGame()
    {
        gameActive = true;
        ball.linearVelocity = Vector2.zero;   // Reiniciar la velocidad
        ball.angularVelocity = 0f;      // Reiniciar la velocidad angular
    }

    // Desactivar el minijuego
    public void DeactivateGame()
    {
        gameActive = false;
    }

    // Método para alinear un objeto con el eje de referencia de la gravedad
    private void AlignWithGravityReference(GameObject obj)
    {
        // Aseguramos que el objeto esté alineado con la rotación del 'gravityReference' (padre)
        obj.transform.rotation = gravityReference.rotation;
    }
}