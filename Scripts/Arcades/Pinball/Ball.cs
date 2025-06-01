using UnityEngine;

public class Ball : MonoBehaviour
{
    public float baseSpeed = 2f;          // Velocidad base de la bola (límite normal)
    public float nitroMultiplier = 1.3f; // Multiplicador en zona Nitro
    public float maxSpeed = 5f;           // Velocidad máxima absoluta permitida

    public int score = 0;                 // Puntaje actual
    public int pointsPerPin = 1;          // Puntos por pin tocado
    public int targetScore = 10;          // Puntaje para ganar
    public string pinTag = "Pinball_Pin"; // Tag que tienen los pines

    private Rigidbody2D rb;

    private bool inNitroZone = false;

    public Game_Pinball gameManager;

    void Start()
    {
        score = 0; // Inicializar puntaje
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!rb) return;

        // Obtener velocidad actual
        Vector2 velocity = rb.linearVelocity;

        // Si está en zona Nitro y va hacia arriba, aumenta velocidad Y
        if (inNitroZone && velocity.y > 0)
        {
            velocity.y = Mathf.Min(velocity.y * nitroMultiplier, maxSpeed);
        }
        else
        {
            // Limitar velocidad Y a baseSpeed (no permitir que suba más que eso)
            if (velocity.y > baseSpeed)
            {
                velocity.y = baseSpeed;
            }
        }

        // Limitar velocidad total a maxSpeed (en ambas direcciones)
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(pinTag))
        {
            score += pointsPerPin;

            if (score >= targetScore)
            {
                if (gameManager != null)
                {
                    gameManager.WinGame();
                    score = 0; // Reiniciar puntaje al ganar
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pinball_Nitro"))
        {
            inNitroZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pinball_Nitro"))
        {
            inNitroZone = false;
        }
    }
}
