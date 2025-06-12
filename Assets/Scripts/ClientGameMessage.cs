using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientGameMessage : MonoBehaviour
{

    [SerializeField] private TMP_Text _chatLog;
    [SerializeField] private TMP_InputField _messageInputField;

    void Start()
    {
        NetworkingClient.OnMessageReceived += appendMessage;
    }

    void OnDestroy()
    {
        NetworkingClient.OnMessageReceived -= appendMessage;
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

    private void appendMessage(string message)
    {
        _chatLog.text += message + "\n";
    }
}
