using UnityEngine;

public class Bullet_SpaceInvaders : MonoBehaviour
{
    public float speed = 10f;
    public bool isEnemy = false;

    void Update()
    {
        Vector3 dir = isEnemy ? Vector3.down : Vector3.up;
        transform.localPosition += dir * speed * Time.deltaTime;

        if (Mathf.Abs(transform.localPosition.y) > 10f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemy && other.CompareTag("Enemy_SpaceInvaders"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (isEnemy && other.CompareTag("Player_SpaceInvaders"))
        {
            Destroy(gameObject);
            // Puedes usar una referencia a Game_SpaceInvaders y llamar LoseGame()
        }
    }
}