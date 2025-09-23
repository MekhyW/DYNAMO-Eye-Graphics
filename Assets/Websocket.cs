using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

public class Websocket : MonoBehaviour
{
    private const int port = 50000;
    private TcpListener listener;

    public Slider xSlider, ySlider, leftEyeClosenessSlider, rightEyeClosenessSlider,
        angrySlider, disgustedSlider, happySlider, neutralSlider, sadSlider, surprisedSlider,
        hypnoticSlider, heartSlider, rainbowSlider, nightmareSlider, gearsSlider, sansSlider, mischievousSlider,
        brightnessSlider;
    public Toggle sillyMode;
    public GameObject vidPlayer;
    public GameObject vidCanvasL, vidCanvasR;

    private bool ValidateMessage(string message)
    {
        string[] terms = message.Split(' ');
        if (terms[0] == "VIDEO" && terms.Length >= 2 && terms.Length <= 3) { return true; }
        if (terms.Length < 13) { return false; }
        for (int i = 0; i < 13; i++)
        {
            if (!float.TryParse(terms[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) { return false; }
            if (value < -1) { return false; }
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
                if (ValidateMessage(message))
                {
                    response = "Acknowledged: " + message;
                    string[] terms = message.Split(' ');
                    if (terms[0] == "VIDEO")
                    {
                        if (terms[1] == "PLAY") 
                        {
                            Debug.Log("VIDEO PLAY " + terms[2]);
                            vidPlayer.GetComponent<VideoPlayer>().url = terms[2];
                            vidPlayer.SetActive(true);
                            vidCanvasL.SetActive(true);
                            vidCanvasR.SetActive(true);
                        }
                        else 
                        {
                            Debug.Log("VIDEO STOP");
                            vidPlayer.SetActive(false);
                            vidCanvasL.SetActive(false);
                            vidCanvasR.SetActive(false);
                        }
                    }
                    else 
                    {
                        xSlider.value = (float.Parse(terms[0], CultureInfo.InvariantCulture) + 1) / 2 * (xSlider.maxValue - xSlider.minValue) + xSlider.minValue;
                        ySlider.value = (float.Parse(terms[1], CultureInfo.InvariantCulture) + 1) / 2 * (ySlider.maxValue - ySlider.minValue) + ySlider.minValue;
                        leftEyeClosenessSlider.value = float.Parse(terms[2], CultureInfo.InvariantCulture);
                        rightEyeClosenessSlider.value = float.Parse(terms[3], CultureInfo.InvariantCulture);
                        angrySlider.value = float.Parse(terms[4], CultureInfo.InvariantCulture);
                        disgustedSlider.value = float.Parse(terms[5], CultureInfo.InvariantCulture);
                        happySlider.value = float.Parse(terms[6], CultureInfo.InvariantCulture);
                        neutralSlider.value = float.Parse(terms[7], CultureInfo.InvariantCulture);
                        sadSlider.value = float.Parse(terms[8], CultureInfo.InvariantCulture);
                        surprisedSlider.value = float.Parse(terms[9], CultureInfo.InvariantCulture);
                        int manual_expression = int.Parse(terms[10], CultureInfo.InvariantCulture);
                        sillyMode.isOn = int.Parse(terms[11], CultureInfo.InvariantCulture) == 1;
                        brightnessSlider.value = float.Parse(terms[12], CultureInfo.InvariantCulture)/100f;
                        hypnoticSlider.value = manual_expression == 6 ? 1 : 0;
                        heartSlider.value = manual_expression == 7 ? 1 : 0;
                        rainbowSlider.value = manual_expression == 8 ? 1 : 0;
                        nightmareSlider.value = manual_expression == 9 ? 1 : 0;
                        gearsSlider.value = manual_expression == 10 ? 1 : 0;
                        sansSlider.value = manual_expression == 11 ? 1 : 0;
                        mischievousSlider.value = manual_expression == 12 ? 1 : 0;
                    }
                }
                else { response = "Invalid message format!"; }
                byte[] responseMessage = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseMessage, 0, responseMessage.Length);
            }
        }
    }

    private async void Start()
    {
        try
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Debug.Log("WebSocket server started on localhost:" + port);
            await StartListening();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to start WebSocket server: " + e.Message);
        }
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
