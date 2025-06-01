using UnityEngine;

public class Arrow : MonoBehaviour
{
    public KeyCode arrowKey;
    private float targetY;
    private float speed;

    public void Init(KeyCode key, float targetY, float speed)
    {
        this.arrowKey = key;
        this.targetY = targetY;
        this.speed = speed;
    }

    void Update()
    {
        if (transform.position.y > targetY + 1.5f)
        {
            Destroy(gameObject);
        }
    }
}
