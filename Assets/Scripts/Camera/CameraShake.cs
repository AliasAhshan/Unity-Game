using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform gameLevel;  // Reference to the "Game Level 1" object (or world objects)
    public float shakeDuration = 0f;  // How long the level should shake
    public float shakeMagnitude = 0.1f;  // Magnitude of the shake

    private Vector3 initialPosition;  // Store the initial position of the "Game Level 1" object
    private float shakeTimer = 0f;  // Timer to control shake duration

    void Start()
    {
        if (gameLevel == null)
        {
            Debug.LogError("Game Level 1 object not assigned! Please assign the 'Game Level 1' object.");
        }

        initialPosition = gameLevel.localPosition;  // Store the initial position of the level
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            // Apply shake to the Game Level 1 object by adding a small random offset
            gameLevel.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            // Reset the Game Level 1 object to its original position when shake ends
            gameLevel.localPosition = initialPosition;
        }
    }

    // Method to trigger shake
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimer = duration;  // Start the shake timer
    }
}
