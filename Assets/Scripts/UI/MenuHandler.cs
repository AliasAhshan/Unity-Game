using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private string startScene;  // Scene to load when starting a new game
    [SerializeField] private string continueScene;  // Scene to load when continuing the game
    [SerializeField] private AudioClip buttonClickSound;  // Sound for button clicks
    [SerializeField] private float buttonClickVolume = 1f;  // Volume for the button click sound
    [SerializeField] private float buttonClickDelay = 0.5f;  // Delay before starting the game to allow sound to play

    private AudioSource audioSource;  // Local AudioSource to handle sounds

    void Start()
    {
        // Add an AudioSource component to this GameObject if one doesn't already exist
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;  // Ensure the AudioSource doesn't play anything on start
    }

    // Quits the game
    public void QuitGame()
    {
        PlayButtonClickSound();
        Debug.Log("Quit Game called.");
        Application.Quit();
    }

    // Loads the scene for continuing the game
    public void LoadScene()
    {
        PlayButtonClickSound();
        Debug.Log("Load Continue Scene called: " + continueScene);
        SceneManager.LoadScene(continueScene);
    }

    // Starts a new game by deleting the save and loading the start scene
    public void StartGame()
    {
        PlayButtonClickSound();
        Debug.Log("Start New Game called.");
        SaveSystem.DeleteSave();

        // Delay scene loading to let the sound play
        StartCoroutine(StartGameWithDelay());
    }

    // Coroutine to handle delayed scene loading
    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(buttonClickDelay);  // Wait for the sound to finish
        SceneManager.LoadScene(startScene);
        Debug.Log("Game has been reset and restarted.");
    }

    // Play the button click sound locally
    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            Debug.Log("Playing button click sound.");
            audioSource.PlayOneShot(buttonClickSound, buttonClickVolume);
        }
        else
        {
            Debug.LogWarning("AudioSource or ButtonClickSound is not set up properly.");
        }
    }
}
