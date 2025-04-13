using UnityEngine;

public class SimpleMinigame : MonoBehaviour
{
    public GameObject ball;              // El objeto de la pelota
    public float moveSpeed = 5f;         // Velocidad de movimiento
    public float gravityStrength = 9.8f; // Fuerza de la gravedad

    private bool gameActive = false;

    void Start()
    {
        // Desactivar el minijuego inicialmente
        DeactivateGame();
    }

    void Update()
    {
        if (!gameActive) return;

        // Movimiento simple de la pelota
        float moveInput = Input.GetAxis("Horizontal");
        ball.transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime);

        // Gravedad que siempre va hacia abajo (independiente de la rotación)
        ball.transform.Translate(Vector3.down * gravityStrength * Time.deltaTime);
    }

    public void ActivateGame()
    {
        gameActive = true;
    }

    public void DeactivateGame()
    {
        gameActive = false;
    }
}
