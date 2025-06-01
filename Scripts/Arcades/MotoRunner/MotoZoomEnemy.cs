using UnityEngine;

public class MotoZoomEnemy : MonoBehaviour
{
    private float speed = 1f;
    private Vector3 startPos;
    private Vector3 endPos;
    private float progress = 0f;
    private Vector3 startScale = Vector3.one * 0.05f;
    private Vector3 endScale = Vector3.one *0.3f;

    public int lane;
    public bool hasFinished = false;

    private Game_MotoZoom gameManager;

    public void Initialize(int laneIndex, Game_MotoZoom manager, float laneDistance)
    {
        lane = laneIndex;
        gameManager = manager;
        startPos = transform.position - new Vector3(0, 0, 0);
        endPos = transform.position - new Vector3((lane - 1) * laneDistance, +1.5f, 0); 
        transform.position = startPos;
        transform.localScale = startScale;
    }

    public void Tick()
    {
        if (hasFinished) return;

        progress += Time.deltaTime * speed;

        transform.position = Vector3.Lerp(startPos, endPos, progress);
        transform.localScale = Vector3.Lerp(startScale, endScale, progress);

        if (progress >= 0.8f)
        {
            hasFinished = true;
        }
    }
}
