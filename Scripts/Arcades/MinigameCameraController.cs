using UnityEngine;

public class MinigameCameraController : MonoBehaviour
{
    public Transform minigameArea;   // El área donde sucede el minijuego
    public Camera minigameCamera;    // La cámara independiente del minijuego

    void Start()
    {
        // Ajustamos la cámara para que siempre vea el área correcta
        minigameCamera.transform.position = new Vector3(minigameArea.position.x, minigameArea.position.y, minigameCamera.transform.position.z);
        minigameCamera.transform.rotation = Quaternion.identity; // Aseguramos que la cámara no se vea afectada por la rotación
    }
}
