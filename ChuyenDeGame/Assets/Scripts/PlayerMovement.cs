using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public CharacterController _CharacterController;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.4f;

    private Vector3 velocity;
    private bool isGrounded;

    [Header("First Person Camera")]
    public Transform cameraHolder; // Empty gameobject xoay cam theo trục X
    public Camera fpsCamera;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (fpsCamera != null)
                fpsCamera.enabled = true;
        }
        else
        {
            if (fpsCamera != null)
                fpsCamera.enabled = false;
        }

        _CharacterController = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
            return;

        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Runner.DeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Runner.DeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        // --- Movement ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;
        _CharacterController.Move(move * Runner.DeltaTime * 5f);

        // --- Gravity ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Runner.DeltaTime;
        _CharacterController.Move(velocity * Runner.DeltaTime);

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
