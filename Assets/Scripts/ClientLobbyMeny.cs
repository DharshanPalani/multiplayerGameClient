using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientLobbyMeny : MonoBehaviour
{
    public void OnStartGamePress()
    {
        NetworkingClient.Instance.StartGame();
    }
}
