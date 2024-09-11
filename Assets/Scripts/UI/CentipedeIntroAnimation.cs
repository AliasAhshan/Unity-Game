using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeIntroAnimation : MonoBehaviour
{
    public Animator centipedeAnimator;  // Reference to the Animator controlling the animation
    public GameObject centipedeIntro; 
    public float animationLength = 3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has collided with the trigger and if the intro has not already played
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;  // Mark as triggered to prevent it from running again

            // Enable the CentipedeIntro GameObject
            centipedeIntro.SetActive(true);

            // Trigger the animation
            centipedeAnimator.SetTrigger("PlayCentipede");

            // Optionally disable after the animation finishes
            StartCoroutine(DisableAfterAnimation());
        }
    }

    private IEnumerator DisableAfterAnimation()
    {
        // Wait for the length of the animation
        yield return new WaitForSeconds(animationLength);

        // Disable the CentipedeIntro GameObject
        centipedeIntro.SetActive(false);
    }
}
