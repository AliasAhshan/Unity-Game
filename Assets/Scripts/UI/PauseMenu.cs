using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Disables the cursor, freezes timeScale, and contains functions that the pause menu buttons can use */

public class PauseMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioClip pressSound;  // Button press sound
    [SerializeField] AudioClip openSound;   // Sound for opening the pause menu
    [SerializeField] float pressSoundVolume = 1f;  // Volume for button press sounds
    [SerializeField] float openSoundVolume = 1f;   // Volume for pause menu open sound

    [Header("Pause Menu Settings")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] float buttonClickDelay = 0.5f;  // Delay for restart and quit actions

    // Use this for initialization
    void OnEnable()
    {
        Cursor.visible = true;
        if (GameManager.Instance.audioSource != null && openSound != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(openSound, openSoundVolume);  // Play open sound with its own volume
        }
        Time.timeScale = 0f;  // Freeze the game
    }

    public void Unpause()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
        if (GameManager.Instance.audioSource != null && pressSound != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(pressSound, pressSoundVolume);  // Play button press sound with its own volume
        }
        Time.timeScale = 1f;  // Resume the game instantly
    }

    public void Quit()
    {
        Time.timeScale = 1f;  // Unpause the game before quitting
        if (GameManager.Instance.audioSource != null && pressSound != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(pressSound, pressSoundVolume);  // Play button press sound
        }
        StartCoroutine(QuitWithDelay());
    }

    public void RestartLevel()
    {
        SaveSystem.DeleteSave();
        Time.timeScale = 1f;  // Unpause the game before restarting
        if (GameManager.Instance.audioSource != null && pressSound != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(pressSound, pressSoundVolume);  // Play button press sound
        }
        StartCoroutine(RestartWithDelay());
    }

    // Coroutine to handle delayed quitting
    private IEnumerator QuitWithDelay()
    {
        yield return new WaitForSecondsRealtime(buttonClickDelay);  // Wait for the sound to play before quitting
        SceneManager.LoadScene("Menu");  // Load the Menu scene after the delay
    }

    // Coroutine to handle delayed restart
    private IEnumerator RestartWithDelay()
    {
        yield return new WaitForSecondsRealtime(buttonClickDelay);  // Wait for the sound to play before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Restart the current scene
    }
}
