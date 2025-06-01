using UnityEngine;

public class PlayerController_SpaceInvaders : MonoBehaviour
{
    public Game_SpaceInvaders gameManager;
    public float speed = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private GameObject currentBullet;

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        transform.localPosition += Vector3.right * move * speed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.localPosition.x, -0.5f, 0.5f);
        transform.localPosition = new Vector3(clampedX, transform.localPosition.y, 0f);

        if (Input.GetKeyDown(KeyCode.Space) && currentBullet == null && gameManager.isGameStarted)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            gameManager.PlaySound(0); // Play bullet sound
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet_SpaceInvaders"))
        {
            gameManager.PlaySound(3); 
            Destroy(other.gameObject);
            gameManager.PlayerHit();
        }
    }
}