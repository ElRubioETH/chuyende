using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class Chat : NetworkBehaviour
{
    public TextMeshProUGUI textMessage;
    public TMP_InputField inputFieldMessage;
    public GameObject button_Chat;




    // chay ngay sau khi nhan vat tham gia tro chs
    public override void Spawned()
    {
        textMessage = GameObject.Find("TextMessage")
            .GetComponent<TextMeshProUGUI>();
        inputFieldMessage = GameObject.Find("InputFieldMessage")
            .GetComponent<TMP_InputField>();
        button_Chat = GameObject.Find("Button");
        button_Chat .GetComponent<Button>()
            .onClick.AddListener(SendMessageChat);
    }
    public void SendMessageChat()
    {
        if (Runner.LocalPlayer == null)
        {
            Debug.LogWarning("LocalPlayer chưa được setup!");
            return;
        }
        var message = inputFieldMessage.text; 
        if (string.IsNullOrWhiteSpace(message)) return;
        var id = Runner.LocalPlayer.PlayerId;
        var text = $" Player {id}:{message}";
        RpcChat(text);
        inputFieldMessage.text = "";

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string msg)
    {
        textMessage.text += msg + "\n";

    }
}
