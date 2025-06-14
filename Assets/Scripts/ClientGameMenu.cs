using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _roomInputField;

    public void OnCreateRoomPress()
    {
        if (validateRoomInput())
        {
            _ = NetworkingClient.Instance.CreateRoom(_roomInputField.text);
            SceneManager.LoadScene(3);
        }
    }

    public void OnJoinRoomPress()
    {
        if (validateRoomInput())
        {
            _ = NetworkingClient.Instance.JoinRoom(_roomInputField.text);
            SceneManager.LoadScene(4);
        }
    }

    public void OnDisconnectPress()
    {
        NetworkingClient.Instance.Disconnect();
        SceneManager.LoadScene(0);
    }

    private bool validateRoomInput()
    {
        if (string.IsNullOrWhiteSpace(_roomInputField.text))
        {
            return false;
        }

        return true;
    } 
}
