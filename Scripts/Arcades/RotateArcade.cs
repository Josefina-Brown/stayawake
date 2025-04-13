using UnityEngine;

public class RotateArcade : MonoBehaviour
{
    public Transform arcadeScreen;  // La pantalla del arcade que se rota
    public float rotationSpeed = 10f; // Velocidad de rotación

    void Update()
    {
        // Rotar la pantalla del arcade, puedes hacerla automática o controlada por el jugador
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            arcadeScreen.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime); // Rotación en el eje Z
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            arcadeScreen.Rotate(Vector3.back * rotationSpeed * Time.deltaTime); // Rotación en el eje Z
        }
    }
}
