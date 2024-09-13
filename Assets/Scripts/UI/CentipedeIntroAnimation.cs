using System.Collections;
using UnityEngine;

public class CentipedeIntroAnimation : MonoBehaviour
{
    public Animator centipedeAnimator;  // Reference to the Animator controlling the animation
    public GameObject centipedeIntro; 
    public float animationLength = 3f;

    public GameObject player; // Reference to the player object
    private PlayerMovement playerMovement; // Reference to the player's movement script
    private Animator playerAnimator; // Reference to the player's animator

    private bool hasTriggered = false;

    private void Start()
    {
        // Get the player's movement script and animator at the start
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has collided with the trigger and if the intro has not already played
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;  // Mark as triggered to prevent it from running again

            // Lock the player's movement
            LockPlayerMovement();

            // Enable the CentipedeIntro GameObject
            centipedeIntro.SetActive(true);

            // Trigger the centipede animation
            centipedeAnimator.SetTrigger("PlayCentipede");

            // Disable the intro after the animation finishes
            StartCoroutine(DisableAfterAnimation());
        }
    }

    private IEnumerator DisableAfterAnimation()
    {
        // Wait for the length of the animation
        yield return new WaitForSeconds(animationLength);

        // Re-enable the player's movement after the animation finishes
        UnlockPlayerMovement();

        // Disable the CentipedeIntro GameObject
        centipedeIntro.SetActive(false);
    }

    // This function locks the player's movement
    private void LockPlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = false; // Assuming the movement script has a canMove variable to control movement
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isRunning", false); // Assuming the running animation is controlled by "isRunning"
            playerAnimator.SetFloat("xVelocity", 0); // Set movement velocity to 0 to stop any movement animations
            playerAnimator.SetFloat("yVelocity", 0); // Set vertical velocity to 0
        }

        // You can also stop the Rigidbody2D's velocity if necessary
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop all movement
        }
    }

    // This function unlocks the player's movement
    private void UnlockPlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = true; // Re-enable movement
        }
    }
}
