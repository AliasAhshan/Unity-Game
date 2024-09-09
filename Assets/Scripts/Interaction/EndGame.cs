using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import the SceneManagement namespace

public class EndGame : MonoBehaviour
{
    // This method is called when the player reaches the end and collides with the object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player has collided with the end level trigger
        if (other.CompareTag("Player"))  // Make sure your player has the tag "Player"
        {
            EndLevel();
        }
    }

    // Method to end the level and load the main menu
    public void EndLevel()
    {
        // Reset time scale to normal in case it was changed (e.g., during pause)
        Time.timeScale = 1f;

        // Load the Menu scene
        SceneManager.LoadScene("Menu");
    }
}
