using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platform; // Reference to the platform GameObject
    public float fallDelay = 1.0f; // Delay before the platform falls

    private Collider2D platformCollider;
    private Rigidbody2D platformRigidbody;

    private void Start()
    {
        if (platform == null)
        {
            Debug.LogError("Platform GameObject not assigned!");
            return;
        }

        platformCollider = platform.GetComponent<Collider2D>();
        platformRigidbody = platform.GetComponent<Rigidbody2D>();

        if (platformRigidbody != null)
        {
            platformRigidbody.bodyType = RigidbodyType2D.Kinematic; // Set to kinematic initially
        }

        // Ignore collisions between the platform and the trigger
        int platformLayer = LayerMask.NameToLayer("Platform");
        int triggerLayer = LayerMask.NameToLayer("Trigger");
        Physics2D.IgnoreLayerCollision(platformLayer, triggerLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Entered by: " + other.name); // Log to check if trigger is detected

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered Trigger"); // Log to check if player is detected
            StartCoroutine(MakePlatformFall());
        }
    }

    private IEnumerator MakePlatformFall()
    {
        yield return new WaitForSeconds(fallDelay); // Wait for the specified delay

        if (platformRigidbody != null)
        {
            platformRigidbody.bodyType = RigidbodyType2D.Dynamic; // Make the platform fall
            Debug.Log("Platform set to dynamic"); // Log to confirm platform is set to dynamic
        }
    }
}