using UnityEngine;

public class Ball_BlockBreaker : MonoBehaviour
{

    public Game_BlockBreaker gameManager; // Referencia al GameManager
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyBlock(GameObject block)
    {
        block.SetActive(false);
        gameManager.GetPoint();
    }


    // Detectar las colisiones con los bloques
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("BlockBreaker_Block"))
        {
            DestroyBlock(collision.gameObject);
        }

        // Vector2 surfaceNormal = collision.contacts[0].normal;
        // ballRb.linearVelocity = Vector2.Reflect(ballRblastVelocity, surfaceNormal);

    }

}
