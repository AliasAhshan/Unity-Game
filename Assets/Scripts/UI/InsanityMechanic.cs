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
            return; // Exit if the mechanic is not enabled
        }

        // Check if the player is moving
        isPlayerMoving = Mathf.Abs(playerMovementScript.horizontalInput) > 0 || Mathf.Abs(playerMovementScript.rb.velocity.y) > 0;

        if (!isPlayerMoving)
        {
            timeStill += Time.deltaTime;
            if (timeStill >= timeBeforeInsanity && !isInsanityActive)
            {
                insanityCoroutine = StartCoroutine(StartInsanity());
            }
        }
        else
        {
            if (isInsanityActive)
            {
                ResetInsanity();
            }

            // Fully reset the timer and the mechanic if the player moves
            StopInsanityEffect();
        }
    }

    private IEnumerator StartInsanity()
    {
        isInsanityActive = true;

        while (insanityOverlay.color.a < 1f)
        {
            // Apply camera shake
            mainCamera.transform.localPosition = originalCameraPosition + Random.insideUnitSphere * cameraShakeAmount;

            // Fade to black
            insanityOverlay.color = new Color(0, 0, 0, insanityOverlay.color.a + fadeSpeed * Time.deltaTime);

            // If the screen is fully black, kill the player
            if (insanityOverlay.color.a >= 1f)
            {
                playerMovementScript.StartCoroutine(playerMovementScript.Die());
            }

            yield return null;
        }
    }

    private void ResetInsanity()
    {
        // Reset insanity and fade effect
        isInsanityActive = false;
        timeStill = 0f; // Fully reset the timer
        insanityOverlay.color = new Color(0, 0, 0, 0); // Reset the overlay to transparent
        mainCamera.transform.localPosition = originalCameraPosition; // Reset the camera position
    }

    private void StopInsanityEffect()
    {
        // Stop the insanity coroutine if it's running and reset everything
        if (insanityCoroutine != null)
        {
            StopCoroutine(insanityCoroutine);
            insanityCoroutine = null; // Clear the reference
        }

        ResetInsanity(); // Ensure everything is reset
    }

    public void EnableInsanityMechanic()
    {
        insanityMechanicEnabled = true; // Enable the insanity mechanic after the save point
    }
}
