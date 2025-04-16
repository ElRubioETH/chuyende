using Fusion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Camera")]
    public CinemachineCamera playerCinemachineCamera; // Changed from VirtualCamera
    public CinemachineCamera FollowCamera;
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("PlayerSetting")]
    public string Name;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public CharacterController _CharacterController;
    public float Speed = 5f;
    public float RunSpeed = 8f;
    public float JumpForce = 8f;
    public float Gravity = 20f;
    private float yVelocity;
    public Animator animator;



    [Networked, OnChangedRender(nameof(OnChangedHealth))]
    public int health { get; set; }
    public int Health { get; private set; }

    [SerializeField] private GameObject cowboyHat;
    [SerializeField] private GameObject cowboyBody;

    private void OnChangedHealth()
    {
        healthText.text = health.ToString();
    }

    public override void Spawned()
    {
        base.Spawned();



        if (Object.HasInputAuthority)
        {
            // Disable all other cameras (safety check)
            var allCams = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var cam in allCams)
            {
                if (cam != playerCinemachineCamera)
                    cam.gameObject.SetActive(false);
            }

            // Configure THIS player's camera
            playerCinemachineCamera.gameObject.SetActive(true);
            playerCinemachineCamera.Follow = cameraPivot;
            playerCinemachineCamera.LookAt = cameraPivot;
            playerCinemachineCamera.Priority.Value = 100; // Highest priority

            // Optional: Hide local player model
            var brain = Camera.main.GetComponent<Unity.Cinemachine.CinemachineBrain>();
            if (brain != null && brain.OutputCamera != null)
            {
                brain.OutputCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LocalOnly"));
            }
        }

        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Name = PlayerPrefs.GetString("PlayerName");
            nameText.text = Name;

            _CharacterController = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();

            RpcUpdateHealth(100);

            // Gán layer LocalOnly cho phần model
            int localOnlyLayer = LayerMask.NameToLayer("LocalOnly");
            SetLayerRecursively(cowboyHat.transform, localOnlyLayer);
            SetLayerRecursively(cowboyBody.transform, localOnlyLayer);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcUpdateHealth(int health)
    {
        Health = health;
        healthText.text = $"{Health}";
    }
    private void ToggleMouseLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMouseLock();
        }
        if (FollowCamera != null)
        {
            nameText.transform.LookAt(FollowCamera.transform);
            healthText.transform.LookAt(FollowCamera.transform);
        }

        if (Object.HasInputAuthority)
        {
            RotateWithMouse();

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            float moveAmount = new Vector2(moveX, moveZ).magnitude;

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : Speed;
            move *= currentSpeed;

            // GRAVITY + JUMP
            if (_CharacterController.isGrounded)
            {
                yVelocity = -1f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yVelocity = JumpForce;

                    // Gọi RPC để sync jump animation
                    RpcPlayJumpAnim();
                }
            }
            else
            {
                yVelocity -= Gravity * Time.deltaTime;
            }

            move.y = yVelocity;
            _CharacterController.Move(move * Time.deltaTime);

            // Gọi RPC để sync Speed anim (Idle/Walk/Run)
            RpcUpdateAnimSpeed(moveAmount);
        }
    }

    void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcPlayJumpAnim()
    {
        if (animator != null)
            animator.SetTrigger("Jump");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcUpdateAnimSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat("Speed", speed);
    }

    private void SetLayerRecursively(Transform obj, int newLayer)
    {
        if (obj == null) return;

        obj.gameObject.layer = newLayer;

        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, newLayer);
        }
    }
}