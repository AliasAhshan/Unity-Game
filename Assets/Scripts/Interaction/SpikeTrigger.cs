using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    public GameObject spikes2; // Reference to the spike GameObject
    public LayerMask playerLayer; // Layer mask for the player

    private void Start()
    {
        if (spikes2 == null)
        {
            Debug.LogError("Spikes 2 GameObject not assigned!");
        }

        // Ignore collisions between the spikes and the trigger
        int spikesLayer = LayerMask.NameToLayer("Spikes");
        int triggerLayer = LayerMask.NameToLayer("Trigger");
        Physics2D.IgnoreLayerCollision(spikesLayer, triggerLayer);

        // Ignore collisions between the spikes and the player
        int playerLayerIndex = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(spikesLayer, playerLayerIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered by: " + other.name); // Log to check if trigger is detected

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered Trigger"); // Log to check if player is detected

            Rigidbody2D rb = spikes2.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // Make the spikes fall
                Debug.Log("Spikes 2 set to dynamic"); // Log to confirm spikes are set to dynamic
            }
            else
            {
                Debug.LogError("Rigidbody2D component not found on Spikes 2!");
            }
        }
    }
}




