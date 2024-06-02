using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collides with an object on the "Void" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Void"))
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle player death (e.g., reload the scene, show game over screen)
        Debug.Log("Player has died!");
        // Example: Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
