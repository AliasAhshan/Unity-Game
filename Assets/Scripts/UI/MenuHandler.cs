using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private string startScene;  // Scene to load when starting a new game
    [SerializeField] private string continueScene;  // Scene to load when continuing the game

    // Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Loads the scene for continuing the game
    public void LoadScene()
    {
        SceneManager.LoadScene(continueScene);
    }

    // Starts a new game by deleting the save and loading the start scene
    public void StartGame()
    {
        // Delete the save file so that the player restarts from the beginning
        SaveSystem.DeleteSave();

        // Load the starting scene for the game
        SceneManager.LoadScene(startScene);

        Debug.Log("Game has been reset and restarted.");
    }
}
