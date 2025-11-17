using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Websockettest : MonoBehaviour
{
    private const int port = 50000;
    private TcpListener listener;

    private bool ValidateMessage(string message)
    {
        string[] terms = message.Split(' ');
        if (terms[0] == "VIDEO" && terms.Length >= 2 && terms.Length <= 3) { return true; }
        if (terms.Length < 14) { Debug.Log("not enough terms"); return false; }
        for (int i = 0; i < 14; i++)
        {
            if (!float.TryParse(terms[i], out float value)) { Debug.Log("value not a number"); return false; }
            if (value < -1) { Debug.Log("value out of range"); return false; }
        }
        return true;
    }

    private async Task StartListening()
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            HandleClient(client);
        }
    }

    private async void HandleClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string response;
                Debug.Log("Received: " + message);
                if (ValidateMessage(message))
                {
                    response = "Acknowledged: " + message;
                }
                else
                {
                    response = "Invalid message format!";
                }
                byte[] responseMessage = Encoding.UTF8.GetBytes(response);
                stream.Write(responseMessage, 0, responseMessage.Length);
            }
        }
    }

    private async void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Debug.Log("WebSocket server started on port " + port);
        await StartListening();
    }

    void Update() {}

    private void OnApplicationQuit()
    {
        if (listener != null)
        {
            listener.Stop();
            Debug.Log("WebSocket server stopped");
        }
    }
}
