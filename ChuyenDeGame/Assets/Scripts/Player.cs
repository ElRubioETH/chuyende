using Fusion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Camera")]
    public CinemachineCamera playerCinemachineCamera; // Changed from VirtualCamera
    public CinemachineCamera FollowCamera;
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("PlayerSetting")]
    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public string Name { get; set; }
    public TextMeshProUGUI nameText;
    public Slider healthSlider;
    public CharacterController _CharacterController;
    public float Speed = 10f;
    public float RunSpeed = 20f;
    public float JumpForce = 8f;
    public float Gravity = 20f;
    private float yVelocity;
    public Animator animator;
    public TMP_InputField mouseadjust;  // Reference to the TMP Input Field



    [Networked, OnChangedRender(nameof(OnChangedHealth))]
    public int health { get; set; }
    public int Health { get; private set; }

    [SerializeField] private GameObject cowboyHat;
    [SerializeField] private GameObject cowboyBody;
    void OnNameChanged()
    {
        if (nameText != null)
            nameText.text = Name;
    }

    void Start()
    {
        mouseadjust = GameObject.Find("mouseadjust")?.GetComponent<TMP_InputField>();
        if (mouseadjust != null)
        {
            mouseadjust.onValueChanged.AddListener(AdjustMouseSensitivity);
            mouseadjust.text = mouseSensitivity.ToString();  // Set initial value
        }
    }
    void AdjustMouseSensitivity(string value)
    {
        if (float.TryParse(value, out float newSensitivity))
        {
            mouseSensitivity = newSensitivity;
        }
    }

    private void OnChangedHealth()
    {
        if (healthSlider != null)
            healthSlider.value = health;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcSetName(string newName)
    {
        Name = newName;
    }
    public override void Spawned()
    {
        base.Spawned();



        if (Object.HasInputAuthority)
        {

            string savedName = PlayerPrefs.GetString("PlayerName");
            RpcSetName(savedName);
            healthSlider = GameObject.Find("PlayerHealth")?.GetComponent<Slider>();
            if (healthSlider != null)
            {
                healthSlider.minValue = 0;
                healthSlider.maxValue = 100;
                healthSlider.value = Health;
            }
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateHealth(int value)
    {
        Health = value;
        health = value;


        OnChangedHealth();
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


        if (Object.HasInputAuthority)
        {
            RotateWithMouse();

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float moveAmount = inputVector.magnitude;

            // Speed check
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : Speed;
            Vector3 move = (transform.right * inputVector.x + transform.forward * inputVector.y) * currentSpeed;
            // GRAVITY + JUMP
            if (_CharacterController.isGrounded)
            {
                yVelocity = -1f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yVelocity = JumpForce;
                    RpcPlayJumpAnim();
                }
            }
            else
            {
                yVelocity -= Gravity * Runner.DeltaTime;
            }

            move.y = yVelocity;
            _CharacterController.Move(move * Runner.DeltaTime);

            // Gọi RPC để sync Speed anim (Idle/Walk/Run)
            RpcUpdateAnimSpeed(moveAmount);
        }
    }

    void RotateWithMouse()
    {
        // Sensitivity multiplier
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply the vertical rotation and clamp it to avoid full rotation (to avoid flipping the camera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Apply horizontal rotation (around the Y-axis)
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