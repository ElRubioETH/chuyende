using UnityEngine;
using Fusion;

public class NetworkedButton : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private MeshRenderer buttonRenderer;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material pressedMaterial;
    private bool isInitialized = false;

    [Networked] private NetworkBool IsPressed { get; set; }

    public override void Spawned()
    {
        Debug.Log("[Button] Spawned called!");
        isInitialized = true;
    }

    private void Start()
    {
        Debug.Log($"[Button] Initializing button: {gameObject.name}");

        if (buttonRenderer == null)
        {
            buttonRenderer = GetComponent<MeshRenderer>();
            Debug.Log($"[Button] Auto-assigned renderer: {buttonRenderer != null}");
        }

        buttonRenderer.material = normalMaterial;
        Debug.Log($"[Button] Initial material set: {normalMaterial != null}");
    }

    public void PressButton()
    {
        if (!isInitialized) return;

        if (Object == null || !Object.IsValid)
        {
            Debug.LogWarning("[Button] Tried to press but NetworkObject not ready (Spawned chưa gọi).");
            return;
        }

        Debug.Log($"[Button] Press attempt - HasAuthority: {HasStateAuthority}, IsPressed: {IsPressed}");

        if (!IsPressed)
            RPC_TriggerPress();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_TriggerPress(RpcInfo info = default)
    {
        Debug.Log($"[Button] RPC received from player {info.Source}");

        if (!IsPressed)
        {
            IsPressed = true;
            RPC_UpdateButton();
            Debug.Log("[Button] Press state updated on server");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateButton()
    {
        Debug.Log($"[Button] Updating visual on client - Pressed: {IsPressed}");

        if (buttonRenderer == null)
        {
            Debug.LogError("[Button] Missing renderer reference!");
            return;
        }

        buttonRenderer.material = IsPressed ? pressedMaterial : normalMaterial;

        if (IsPressed)
        {
            var train = FindObjectOfType<TrainAnimationController>();
            Debug.Log($"[Button] Train controller found: {train != null}");
            train?.PlayAnimation();
        }
    }
}