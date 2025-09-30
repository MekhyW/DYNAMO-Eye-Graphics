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
    
    /// <summary>
    /// Plays a video with sound once, then loops it muted
    /// </summary>
    /// <param name="url">The URL or path of the video to play</param>
    public void PlayVideoWithSoundOnce(string url)
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found!");
            return;
        }
        StopVideo();
        currentUrl = url;
        hasPlayedOnce = false;
        videoPlayer.url = url;
        videoPlayer.isLooping = false;
        videoPlayer.SetDirectAudioVolume(0, initialVolume);
        gameObject.SetActive(true);
        ActivateCanvases(true);
        videoPlayer.Prepare();
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
        gameObject.SetActive(false);
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