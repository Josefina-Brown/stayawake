using UnityEngine;

public class SimplePinball : MonoBehaviour
{
    public Rigidbody2D ballRb;
    public Transform spawnPoint;
    public Collider2D[] walls;
    public Collider2D bumper;
    public float bumperForce = 10f;
    public float gravityScale = 1f;

    public Transform bumperTransform;
    public Vector2 bumperMoveDirection = Vector2.up;
    public float bumperMoveDistance = 1f;
    public float bumperMoveSpeed = 5f;

    private Vector3 bumperStartPos;
    private bool isMovingBumper = false;

    void Start()
    {
        ballRb.gravityScale = gravityScale;
        ResetBall();
        if (bumperTransform != null)
            bumperStartPos = bumperTransform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetBall();

        if (Input.GetKeyDown(KeyCode.Space) && !isMovingBumper)
            StartCoroutine(MoveBumper());
    }

    void ResetBall()
    {
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        ballRb.transform.position = spawnPoint.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == bumper)
        {
            Vector2 forceDir = (collision.transform.position - bumper.transform.position).normalized;
            ballRb.AddForce(forceDir * bumperForce, ForceMode2D.Impulse);
        }
    }

    System.Collections.IEnumerator MoveBumper()
    {
        isMovingBumper = true;

        Vector3 start = bumperStartPos;
        Vector3 target = start + (Vector3)(bumperMoveDirection.normalized * bumperMoveDistance);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bumperMoveSpeed;
            bumperTransform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * bumperMoveSpeed;
            bumperTransform.localPosition = Vector3.Lerp(target, start, t);
            yield return null;
        }

        bumperTransform.localPosition = bumperStartPos;
        isMovingBumper = false;
    }
}
