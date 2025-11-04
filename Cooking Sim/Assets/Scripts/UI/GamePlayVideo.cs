using UnityEngine;
using UnityEngine.Video;
public class GamePlayVideo : MonoBehaviour

{
    public VideoPlayer videoPlayer; // Assign in Inspector

    public void StartGame()
    {
        // Load next scene or hide menu
        // SceneManager.LoadScene("GameScene"); // if using multiple scenes

        // Start video playback
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}