using UnityEngine;

public class Shield : MonoBehaviour
{
    public int maxHits = 3;         // Cuántos impactos puede recibir
    private int currentHits = 0;    // Contador actual de impactos

    public Sprite[] damageSprites; // Opcional: sprites que cambian según el daño

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet_SpaceInvaders") || other.CompareTag("Bullet_SpaceInvaders"))
        {
            Destroy(other.gameObject);
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        currentHits++;

        // if (damageSprites.Length > 0 && currentHits - 1 < damageSprites.Length)
        // {
        //     spriteRenderer.sprite = damageSprites[currentHits - 1];
        // }

        if (currentHits >= maxHits)
        {
            Destroy(gameObject);
        }
    }
}
