using UnityEngine;
using Fusion;

public class PlayerInteraction : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;
    private NetworkedButton currentButton;

    private void Start()
    {
        Debug.Log($"[Player] Interaction system initialized on {gameObject.name}");
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            Debug.Log("[Player] No state authority, skipping input");
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[Player] E key pressed");

            if (currentButton != null)
            {
                Debug.Log($"[Player] Attempting to press button: {currentButton.gameObject.name}");
                currentButton.PressButton();
            }
            else
            {
                Debug.Log("[Player] No button in range");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        Debug.Log($"[Player] Entered trigger: {other.gameObject.name}");

        if (other.CompareTag("Button"))
        {
            if (other.TryGetComponent(out NetworkedButton button))
            {
                currentButton = button;
                Debug.Log($"[Player] Valid button found: {other.gameObject.name}");
            }
            else
            {
                Debug.LogError($"[Player] Object tagged 'Button' missing NetworkedButton component: {other.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.CompareTag("Button") && currentButton != null && other.gameObject == currentButton.gameObject)
        {
            Debug.Log($"[Player] Exited button range: {other.gameObject.name}");
            currentButton = null;
        }
    }
}