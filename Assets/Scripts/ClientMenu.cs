using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInputField;

    public void OnConnectPress()
    {
        if (string.IsNullOrWhiteSpace(_usernameInputField.text))
        {
            Debug.Log("Enter a name daw!");
        }
        else
        {            
            NetworkingClient.Instance.Connect(_usernameInputField.text);
        }

    }
}
