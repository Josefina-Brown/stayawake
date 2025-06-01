using UnityEngine;

public class MotoElement : MonoBehaviour
{
    public float speed = 0.5f;
    public bool isStar = false;

    void Update()
    {
        transform.position += new Vector3(1,0,0) * speed * Time.deltaTime;

        if (transform.position.x > -40f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //GameObject.FindObjectOfType<Game_MotoRunner>().RegisterCollision(isStar);
            Destroy(gameObject);
        }
    }
}
