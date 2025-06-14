using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingClient : MonoBehaviour
{
    public static NetworkingClient Instance { get; private set; }
    public static event Action<string> OnMessageReceived;
    public static event Action<List<string>> OnClientJoin;

    private ClientWebSocket clientSocket;
    private CancellationTokenSource cancelSource;
    public string uri = "ws://localhost:3000";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void Connect(string username)
    {
        clientSocket = new ClientWebSocket();
        cancelSource = new CancellationTokenSource();

        try
        {
            await clientSocket.ConnectAsync(new Uri(uri), cancelSource.Token);
            await SendJson($"{{\"type\":\"set_username\",\"username\":\"{username}\"}}");
            _ = ReceiveLoop();
            SceneManager.LoadScene(1);
        }
        catch (Exception ex)
        {
            Debug.LogError("WebSocket connection error: " + ex.Message);
        }
    }

    public async void Disconnect()
    {
        if (clientSocket == null || clientSocket.State != WebSocketState.Open)
            return;

        try
        {
            cancelSource?.Cancel();

            await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            clientSocket.Dispose();
            clientSocket = null;

            cancelSource.Dispose();
            cancelSource = null;

            SceneManager.LoadScene(0);
        }
        catch (Exception ex)
        {
            Debug.Log("Error disconnecting: " + ex.Message);
        }
    }

    public async Task CreateRoom(string roomName)
    {
        await SendJson($"{{\"type\":\"create_room\",\"room\":\"{roomName}\"}}");
    }

    public async Task JoinRoom(string roomName)
    {
        await SendJson($"{{\"type\":\"join_room\",\"room\":\"{roomName}\"}}");
    }

    public async Task SendChatMessage(string text)
    {
        await SendJson($"{{\"type\":\"chat\",\"message\":\"{text}\"}}");
    }
    public async void StartGame(string text = "host")
    {
        await SendJson($"{{\"type\":\"request_start\",\"host\":\"{text}\"}}");
    }

    private async Task SendJson(string json)
    {
        if (clientSocket == null || clientSocket.State != WebSocketState.Open)
            return;

        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await clientSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancelSource.Token);
    }

    private async Task ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (clientSocket != null && clientSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancelSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.None);
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var data = JsonUtility.FromJson<ChatPacket>(message);
                if (data.type == "chat")
                {
                    OnMessageReceived?.Invoke($"{data.username}: {data.message}");
                }
                else if (data.type == "client_list")
                {
                    var clientList = JsonUtility.FromJson<ClientListPacket>(message);

                    List<string> usernames = new List<string>();
                    foreach (var client in clientList.clients)
                    {
                        usernames.Add(client.username);

                    }
                    OnClientJoin?.Invoke(usernames);
                }
                else if (data.type == "draw")
                {
                    var draw = JsonUtility.FromJson<DrawPacket>(message);

                    DrawingBoard board = FindObjectOfType<DrawingBoard>();
                    if (board != null)
                    {
                        Color color;
                        ColorUtility.TryParseHtmlString(draw.color, out color);
                        board.DrawRemoteStroke(
                            new Vector2(draw.x0, draw.y0),
                            new Vector2(draw.x1, draw.y1),
                            color,
                            draw.size
                        );
                    }
                }
                else if (data.type == "start_game")
                {
                    SceneManager.LoadScene(2);
                }

            }
            catch (Exception ex)
            {
                Debug.LogError("Receive error: " + ex.Message);
                break;
            }
        }
    }

    public async Task RequestClientList()
    {
        await SendJson("{\"type\":\"get_clients\"}");
    }

    public void SendDrawData(Vector2 from, Vector2 to, Color color, int size)
    {
        DrawPacket packet = new DrawPacket
        {
            x0 = from.x,
            y0 = from.y,
            x1 = to.x,
            y1 = to.y,
            color = "#" + ColorUtility.ToHtmlStringRGB(color),
            size = size
        };

        string json = JsonUtility.ToJson(packet);
        _ = SendJson(json);
    }



    [Serializable]
    private class ChatPacket
    {
        public string type;
        public string username;
        public string message;
    }

    [Serializable]
    private class ClientListPacket
    {
        public string type;
        public string room;
        public ClientInfo[] clients;
    }

    [Serializable]
    public class DrawPacket
    {
        public string type = "draw";
        public float x0, y0, x1, y1;
        public string color;
        public int size;
    }


    [Serializable]
    private class ClientInfo
    {
        public string username;
        public bool hasGuessedWord;
    }


    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
