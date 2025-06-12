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

}
