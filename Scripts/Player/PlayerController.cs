using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float bobFrequency = 1.5f;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0f;
    private Vector3 initialCameraPos;

    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float yVelocity = 0f;

    private bool isFrozen = false;

    void Start()
    {
        initialCameraPos = cameraTransform.localPosition;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isFrozen)
        {

            return; // Evita cualquier entrada o movimiento
        }

        // Movimiento
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Gravedad
        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        // if (Input.GetButtonDown("Jump") && controller.isGrounded)
        // {
        //     yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // }

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Cámara
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        ApplyHeadBob(moveX, moveZ);
    }

    void ApplyHeadBob(float moveX, float moveZ)
    {
        if (controller.isGrounded && (moveX != 0 || moveZ != 0))
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
            cameraTransform.localPosition = initialCameraPos + new Vector3(0, bobOffset, 0);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCameraPos, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }

    // ✅ Congela al jugador
    public void FreezePlayer()
    {
        isFrozen = true;
        yVelocity = 0f;
        controller.enabled = false;
        Cursor.lockState = CursorLockMode.None; // opcional: desbloquea el cursor
    }

    // ✅ Lo descongela
    public void UnfreezePlayer()
    {
        isFrozen = false;
        controller.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; // opcional: vuelve a bloquear el cursor
    }


}
