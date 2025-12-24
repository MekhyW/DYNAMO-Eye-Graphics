using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoPlayerController : MonoBehaviour
{
    [Header("Video Player Settings")]
    public VideoPlayer videoPlayer;
    public GameObject[] canvases;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float initialVolume = 1f;
    
    private bool hasPlayedOnce = false;
    private bool isCurrentlyPlaying = false;
    private string currentUrl = "";
    
    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    IEnumerator DownloadAndPlay(string url, bool retryOnFailure = true)
    {
        if (System.IO.Directory.Exists(Application.temporaryCachePath))
        {
            try { System.IO.Directory.Delete(Application.temporaryCachePath, true); }
            catch (System.Exception e) { Debug.LogWarning($"Could not delete cache directory: {e.Message}"); }
        }
        if (!System.IO.Directory.Exists(Application.temporaryCachePath)) { System.IO.Directory.CreateDirectory(Application.temporaryCachePath); }
        string fileName = System.IO.Path.GetFileName(url);
        string localPath = System.IO.Path.Combine(Application.temporaryCachePath, fileName);
        using (var req = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            req.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(localPath);
            req.timeout = 30;
            yield return req.SendWebRequest();
            if (req.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                if (retryOnFailure && req.responseCode == 404)
                {
                    Debug.LogWarning($"Video not found (404): {url}. Retrying in 5 seconds...");
                    yield return new WaitForSeconds(5f);
                    yield return StartCoroutine(DownloadAndPlay(url, false));
                    yield break;
                }
                Debug.LogError($"Video download failed: {req.error}");
                yield break;
            }
        }
        PlayLocalVideo(localPath);
    }

    void PlayLocalVideo(string localPath)
    {
        StopVideo();
        currentUrl = localPath;
        hasPlayedOnce = false;
        videoPlayer.url = localPath;
        videoPlayer.isLooping = false;
        videoPlayer.SetDirectAudioVolume(0, initialVolume);
        ActivateCanvases(true);
        videoPlayer.Prepare();
    }
    
    /// <summary>
    /// Plays a video with sound once, then loops it muted
    /// </summary>
    /// <param name="url">The URL or path of the video to play</param>
    public void PlayVideoWithSoundOnce(string url)
    {
        StartCoroutine(DownloadAndPlay(url));
    }
    
    /// <summary>
    /// Stops the video and deactivates canvases
    /// </summary>
    public void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying) videoPlayer.Stop();
        isCurrentlyPlaying = false;
        hasPlayedOnce = false;
        ActivateCanvases(false);
    }
    
    /// <summary>
    /// Called when video preparation is complete
    /// </summary>
    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared, starting playback with sound");
        videoPlayer.Play();
        isCurrentlyPlaying = true;
    }
    
    /// <summary>
    /// Called when video reaches the end
    /// </summary>
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!hasPlayedOnce)
        {
            Debug.Log("First playback finished, starting muted loop");
            hasPlayedOnce = true;
            videoPlayer.isLooping = true;
            videoPlayer.SetDirectAudioVolume(0, 0f);
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
    }
    
    /// <summary>
    /// Activates or deactivates the specified canvases
    /// </summary>
    /// <param name="activate">True to activate, false to deactivate</param>
    private void ActivateCanvases(bool activate)
    {
        if (canvases != null)
        {
            foreach (GameObject canvas in canvases)
            {
                if (canvas != null) canvas.SetActive(activate);
            }
        }
    }
    
    /// <summary>
    /// Check if video is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return isCurrentlyPlaying && videoPlayer != null && videoPlayer.isPlaying;
    }
    
    /// <summary>
    /// Get the current video URL
    /// </summary>
    public string GetCurrentUrl()
    {
        return currentUrl;
    }
    
    /// <summary>
    /// Set the volume for the initial playback
    /// </summary>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetInitialVolume(float volume)
    {
        initialVolume = Mathf.Clamp01(volume);
    }
}