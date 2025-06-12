using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSide : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_InputField _roomInputField;
    [SerializeField] private TMP_InputField _messageInputField;
    [SerializeField] private TMP_Text _chatLog;
    [SerializeField] private TMP_Text _clientList;


    public string username = "Sybau";

    private void Start()
    {
        // NetworkingClient.OnMessageReceived += AppendChat;
        NetworkingClient.OnClientJoin += AppendClientName;
    }

    private void OnDestroy()
    {
        // NetworkingClient.OnMessageReceived -= AppendChat;
        NetworkingClient.OnClientJoin -= AppendClientName;
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

    

    public void OnDisconnectPress()
    {
        NetworkingClient.Instance.Disconnect();
    }

    public void OnCreateRoomPress()
    {
        if (string.IsNullOrWhiteSpace(_roomInputField.text))
        {
            Debug.Log("Enter a room name");
        }

        string roomname = _roomInputField.text;
        _roomInputField.text = "";

        _ = NetworkingClient.Instance.CreateRoom(roomname);

        SceneManager.LoadScene(2);
    }

    public void OnJoinRoomPress()
    {
        if (string.IsNullOrWhiteSpace(_roomInputField.text))
        {
            Debug.Log("Enter a room name");
        }

        string roomname = _roomInputField.text;
        _roomInputField.text = "";

        _ = NetworkingClient.Instance.JoinRoom(roomname);

        SceneManager.LoadScene(2);
    }

    private void AppendChat(string msg)
    {
        _chatLog.text += msg + "\n";
    }

    private void AppendClientName(List<string> usernames)
    {
        _clientList.text = "";

        foreach (var username in usernames)
        {
            _clientList.text += username + "\n";
        }
    }
}
