using TMPro;
using UnityEngine;

public class ClientSide : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_InputField _messageInputField;
    [SerializeField] private TMP_Text _chatLog;

    public string username = "Sybau";

    private void Start()
    {
        NetworkingClient.OnMessageReceived += AppendChat;
    }

    private void OnDestroy()
    {
        NetworkingClient.OnMessageReceived -= AppendChat;
    }

    public void OnConnectPress()
    {
        username = _inputField.text;
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.Log("Enter a username.");
            return;
        }

        NetworkingClient.Instance.Connect(username);
    }

    public void OnSendMessagePress()
    {
        if (string.IsNullOrWhiteSpace(_messageInputField.text))
        {
            Debug.Log("Enter a msg.");
            return;
        }

        string message = _messageInputField.text;
        _messageInputField.text = "";

        _ = NetworkingClient.Instance.SendChatMessage(message);
    }

    public void OnDisconnectPress()
    {
        NetworkingClient.Instance.Disconnect();
    }

    public void OnCreateRoomPress()
    {
        _ = NetworkingClient.Instance.CreateRoom("Diddy party");
    }

    private void AppendChat(string msg)
    {
        _chatLog.text += msg + "\n";
    }
}
