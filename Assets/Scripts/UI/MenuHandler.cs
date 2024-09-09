using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Allows buttons to fire various functions, like QuitGame and LoadScene*/

public class MenuHandler : MonoBehaviour {

	[SerializeField] private string whichScene;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(whichScene);
    }

    public void StartGame()
    {
        // Delete the save file so that the player restarts from the beginning
        SaveSystem.DeleteSave();

        // Reload the scene
        SceneManager.LoadScene(whichScene);


        Debug.Log("Game has been reset and restarted.");
    }

}
