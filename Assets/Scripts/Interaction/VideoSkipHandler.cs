using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSkipHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Reference to the VideoPlayer
    public string nextScene;         // The scene to load after the video or if skipped

    void Start()
    {
        // Ensure the video plays from the start
        videoPlayer.Play();

        // Subscribe to the end of the video event to automatically load the next scene
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        // Check for user input to skip the video (e.g., pressing any key or a specific button)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SkipVideo();
        }
    }

    // Method to skip the video and load the next scene
    public void SkipVideo()
    {
        videoPlayer.Stop();  // Stop the video
        SceneManager.LoadScene(nextScene);  // Load the next scene
    }

    // Method that gets called when the video finishes playing
    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);  // Load the next scene after the video ends
    }
}
