using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class InsanityMechanic : MonoBehaviour
{
    public float timeBeforeInsanity = 3f; // Time before the insanity effect starts
    private float timeStill = 0f; // Time the player has been still
    private bool isPlayerMoving = true;
    private bool insanityMechanicEnabled = false; // Flag to enable insanity mechanic

    public Image insanityOverlay; // Overlay to show the insanity effect (fades to black)
    public float fadeSpeed = 1f; // Speed of the fade effect
    private bool isInsanityActive = false;

    public Camera mainCamera; // Reference to the main camera
    public float cameraShakeAmount = 0.05f; // Amount of camera shake
    private Vector3 originalCameraPosition;

    private PlayerMovement playerMovementScript;
    private Coroutine insanityCoroutine; // Keep track of the insanity coroutine

    public AudioSource heartbeatAudioSource; // AudioSource component to play heartbeat sound
    public AudioClip heartbeatSound; // Heartbeat sound clip
    public float maxVolume = 1f;  // The maximum volume for the heartbeat sound
    public float volumeFadeSpeed = 0.5f;  // Speed at which the volume increases



    void Start()
    {
        originalCameraPosition = mainCamera.transform.localPosition;
        insanityOverlay.color = new Color(0, 0, 0, 0); // Start with a transparent overlay
        insanityOverlay.raycastTarget = false; // Disable raycast target on the overlay
        playerMovementScript = GetComponent<PlayerMovement>();


    }

    void Update()
    {
        if (!insanityMechanicEnabled)
        {
            Debug.Log("Insanity Mechanic is disabled.");
            //return; // Exit if the mechanic is not enabled
        }

        // Check if the player is moving
        isPlayerMoving = Mathf.Abs(playerMovementScript.horizontalInput) > 0 || Mathf.Abs(playerMovementScript.rb.velocity.y) > 0;

        if (!isPlayerMoving)
        {
            Debug.Log("Player is still. Time still: " + timeStill);
            timeStill += Time.deltaTime;  // Increase the timer when player is still
            if (timeStill >= timeBeforeInsanity && !isInsanityActive)
            {
                // Start the insanity coroutine
                Debug.Log("Starting Insanity Mechanic.");
                insanityCoroutine = StartCoroutine(StartInsanity());
            }
        }
        else
        {
            if (isInsanityActive)
            {
                Debug.Log("Player moved, resetting insanity.");
                ResetInsanity();  // Fully reset the insanity when the player moves
            }
            
            StopInsanityEffect();  // Ensure the insanity effect is stopped when the player moves
        }
    }





    private IEnumerator StartInsanity()
    {
        isInsanityActive = true;

        // Start the heartbeat sound if it's not already playing
        if (heartbeatAudioSource != null && heartbeatSound != null)
        {
            if (!heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.clip = heartbeatSound; // Ensure the right clip is set
                heartbeatAudioSource.volume = 0f;  // Start the volume at 0
                heartbeatAudioSource.Play();  // Play the heartbeat sound
            }
        }
        else
        {
            Debug.LogWarning("Heartbeat AudioSource or sound is missing!");
        }

        // Store the initial position of the overlay to reset after shake
        Vector3 initialOverlayPosition = insanityOverlay.transform.localPosition;

        // Set a reasonable shake limit
        float shakeLimit = 100f;

        // Gradually increase the heartbeat volume as the insanity overlay fades in
        while (insanityOverlay.color.a < 1f)
        {
            // Apply shake to the overlay with controlled shake magnitude
            Vector3 shakeOffset = (Vector3)(Random.insideUnitCircle * cameraShakeAmount);

            shakeOffset.x = Mathf.Clamp(shakeOffset.x, -shakeLimit, shakeLimit);
            shakeOffset.y = Mathf.Clamp(shakeOffset.y, -shakeLimit, shakeLimit);

            // Apply the clamped shake offset to the overlay
            insanityOverlay.transform.localPosition = initialOverlayPosition + shakeOffset;

            // Gradually increase the volume of the heartbeat
            heartbeatAudioSource.volume = Mathf.Lerp(heartbeatAudioSource.volume, maxVolume, volumeFadeSpeed * Time.deltaTime);

            // Fade to black
            insanityOverlay.color = new Color(0, 0, 0, insanityOverlay.color.a + fadeSpeed * Time.deltaTime);

            // If the screen is fully black, kill the player
            if (insanityOverlay.color.a >= 1f)
            {
                playerMovementScript.StartCoroutine(playerMovementScript.Die());
                break;
            }

            yield return null;
        }

        // Reset the overlay position after the insanity ends
        insanityOverlay.transform.localPosition = initialOverlayPosition;

        // Reset the overlay alpha
        insanityOverlay.color = new Color(0, 0, 0, 0);
    }



    public void ResetInsanity()
    {
        // Reset insanity-related variables but don't disable the mechanic
        isInsanityActive = false;
        timeStill = 0f;
        insanityOverlay.color = new Color(0, 0, 0, 0); // Reset overlay to transparent
        mainCamera.transform.localPosition = originalCameraPosition; // Reset camera shake
        
        // Do NOT disable the mechanic here
        // insanityMechanicEnabled = false; // Removing this line to keep the mechanic active
    }




    public void StopInsanityEffect()
    {
        if (insanityCoroutine != null)
        {
            StopCoroutine(insanityCoroutine); // Stop insanity coroutine
            insanityCoroutine = null;  // Clear reference
        }

        // Stop the heartbeat sound if it's playing
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
        {
            heartbeatAudioSource.Stop(); // Stop the heartbeat immediately
        }

        isInsanityActive = false;
        timeStill = 0f; // Reset the timer
        insanityOverlay.color = new Color(0, 0, 0, 0);  // Reset the insanity overlay without affecting UI
    }

    public void EnableInsanityMechanic()
    {
        insanityMechanicEnabled = true;  // Enable the insanity mechanic
        isInsanityActive = false;  // Ensure the insanity is not active at the start
        timeStill = 0f;  // Reset the timeStill timer
        if (insanityCoroutine != null)
        {
            //StopCoroutine(insanityCoroutine);  // Stop any running insanity coroutine
            insanityCoroutine = null;
        }
        insanityOverlay.color = new Color(0, 0, 0, 0);  // Ensure the overlay is reset to fully transparent
        mainCamera.transform.localPosition = originalCameraPosition;  // Reset camera shake
        Debug.Log("Insanity mechanic enabled and reset.");
    }



}
