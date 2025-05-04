using UnityEngine;

public class Enemy_SpaceInvader : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public float shootChancePerSecond = 0.1f;

    public bool isAlive = true;

    void Update()
    {
        if (!isAlive) return;

        if (Random.value < shootChancePerSecond * Time.deltaTime)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity)
            .GetComponent<Bullet_SpaceInvaders>().isEnemy = true;
    }

    public void Kill()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet_SpaceInvaders"))
        {
            Destroy(other.gameObject);
            Kill();
        }
    }
}
