using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ClientUsernameDisplay_Lobby : MonoBehaviour
{
    public TMP_Text _clientList;

    async void Update()
    {
        await NetworkingClient.Instance.RequestClientList();
    }

    void Start()
    {
        NetworkingClient.OnClientJoin += AppendClientName;
    }

    void OnDestroy()
    {
        NetworkingClient.OnClientJoin -= AppendClientName;
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
