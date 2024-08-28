using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed of the player
    public float jumpPower = 5f; // Jump power of the player

    float horizontalInput;
    bool isFacingRight = true;
    bool isGrounded = false;

    Rigidbody2D rb;
    Animator animator;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Death Settings")]
    public ParticleSystem deathParticles;  // Particle system for death effect
    public AudioClip deathSound;
    public AudioSource audioSource; // This should point to the Player 2 AudioSource

    [Header("Sound Settings")]
    public AudioClip jumpSound;     // Sound for jumping
    public AudioClip landSound;     // Sound for landing
    public AudioClip footstepSound; // Single sound for each footstep

    public bool dead = false;

    //private float footstepTimer = 0f; // Timer to control the footstep rhythm
    //public float footstepInterval = 0.5f; // Time interval between each footstep sound

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        Debug.Log("Player started with health: " + currentHealth);

        // Ensure audioSource is set, even if it's not assigned in the Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource component found on Player 2. Please assign it in the inspector or add an AudioSource component.");
            }
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Check if the player is on the ground and handle the jumping animation
        if (isGrounded && rb.velocity.y <= 0)  // Adjusting the grounded state when y velocity is 0 or negative
        {
            animator.SetBool("isJumping", false);
        }
        else if (!isGrounded && rb.velocity.y > 0)  // Set jump animation when in the air and moving upward
        {
            animator.SetBool("isJumping", true);
        }

        // Check for jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);  // Perform the jump
            isGrounded = false;  // Mark as not grounded
            animator.SetBool("isJumping", true);  // Set jump animation
            PlayJumpSound();  // Play jump sound
            Debug.Log("Player jumped");
        }

        // Flip sprite direction based on movement
        FlipSprite();

        // Handle footstep sounds
        //HandleFootstepSound();

        // Pause the game
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        // Check if dead and reload scene
        if (dead && Input.GetButtonDown("Jump"))
        {
            ReloadScene();
        }
    }

    private void FixedUpdate()
    {
        // Check if the player is pushing against a wall
        bool isPushingAgainstWall = IsPushingAgainstWall();

        // Handle horizontal movement
        if (isPushingAgainstWall && Mathf.Abs(rb.velocity.y) > 0) // Only restrict horizontal movement when touching and moving against a wall
        {
            // Player is pushing against a wall, prevent horizontal movement to allow sliding
            rb.velocity = new Vector2(0, rb.velocity.y); // Set horizontal velocity to 0 to prevent sticking
        }
        else
        {
            // Normal horizontal movement
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        // Set animator velocity parameters
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        // Update the yVelocity parameter in the animator based on the current vertical velocity
        animator.SetFloat("yVelocity", rb.velocity.y);

        // Simulate friction by slightly reducing horizontal velocity
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.x *= 0.98f;
        rb.velocity = currentVelocity;

        // Check if the player is grounded
        if (rb.velocity.y <= 0 && isGroundedCheck())
        {
            isGrounded = true;
            animator.SetBool("isJumping", false); // Disable jumping animation when grounded
        }
        else
        {
            isGrounded = false;
            animator.SetBool("isJumping", true);  // Enable jumping animation when in the air or sliding
        }
    }


    private bool IsPushingAgainstWall()
    {
        // Cast a ray horizontally from the character's position to check for walls
        RaycastHit2D wallCheckLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.5f, LayerMask.GetMask("Ground"));
        RaycastHit2D wallCheckRight = Physics2D.Raycast(transform.position, Vector2.right, 0.5f, LayerMask.GetMask("Ground"));

        // Use surface normal to detect walls (vertical surfaces)
        if (wallCheckLeft.collider != null && Mathf.Abs(wallCheckLeft.normal.x) > 0.7f)
        {
            return true; // Left wall detected
        }

        if (wallCheckRight.collider != null && Mathf.Abs(wallCheckRight.normal.x) > 0.7f)
        {
            return true; // Right wall detected
        }

        return false; // No wall detected
    }






    // A method to check if the player is grounded using a ground check or collision
    private bool isGroundedCheck()
    {
        Collider2D collider = GetComponent<BoxCollider2D>(); // Use the active collider

        // Get the bottom of the collider (subtract collider's half height from the position)
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y - collider.bounds.extents.y, transform.position.z);

        // Set a small raycast length (ensure it's enough to reach the ground)
        float rayLength = 2f;

        // Cast the ray slightly below the character's collider
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayLength, LayerMask.GetMask("Ground"));

        Debug.DrawRay(rayStart, Vector2.down * rayLength, Color.red); // Optional: Visualize the raycast in Scene view

        return hit.collider != null; // Return true if the ray hits a collider (ground)
    }


    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        isGrounded = true;
        animator.SetBool("isJumping", !isGrounded);

        if (collision.gameObject.CompareTag("Spike"))
        {
            Debug.Log("Player collided with a spike");
            if (!dead)
            {
                StartCoroutine(Die());
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Void"))
        {
            Debug.Log("Player entered void");
            StartCoroutine(Die());
        }

        PlayLandSound(); // Play land sound after landing
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        Debug.Log("Pause menu toggled. Active: " + pauseMenu.activeSelf);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Current Health: " + currentHealth);

        if (currentHealth <= 0 && !dead)
        {
            Debug.Log("Player should die now");
            StartCoroutine(Die());
        }
    }

    public void Hide(bool hide)
    {
        // Assuming you have a SpriteRenderer on your player
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = !hide;  // Disable all sprites if hide is true
        }
    }


    public IEnumerator Die()
    {
        if (!dead)
        {
            dead = true;

            // Emit death particles
            if (deathParticles != null)
            {
                deathParticles.Emit(30);  // Emit 10 particles
            }
            else
            {
                Debug.LogWarning("Death particles not assigned");
            }

            // Play death sound
            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);  // Play death sound
            }
            else
            {
                Debug.LogWarning("Audio source or death sound not assigned");
            }

            // Hide the player (you can create your own Hide function)
            Hide(true);  // This function will hide the player from view

            // Slow down time for effect
            Time.timeScale = 0.6f;

            // Wait for 2 seconds (in real time)
            yield return new WaitForSecondsRealtime(2f);

            // Reset time scale back to normal
            Time.timeScale = 1f;

            // Trigger any UI animations for covering the screen, if needed
            // GameManager.Instance.hud.animator.SetTrigger("coverScreen");

            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void ReloadScene()
    {
        Debug.Log("Scene reloading");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Audio-related methods

    void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound); // Play jump sound
            Debug.Log("Jump sound played");
        }
        else
        {
            Debug.LogWarning("Audio source or jump sound not assigned");
        }
    }

    void PlayLandSound()
    {
        if (audioSource != null && landSound != null && !isGrounded)
        {
            audioSource.PlayOneShot(landSound); // Play landing sound
            Debug.Log("Land sound played");
        }
        else
        {
            Debug.LogWarning("Audio source or land sound not assigned");
        }
    }

    // void HandleFootstepSound()
    // {
    //     // Play footstep sound at regular intervals when grounded and moving
    //     if (Mathf.Abs(horizontalInput) > 0 && isGrounded)
    //     {
    //         footstepTimer -= Time.deltaTime;
    //         if (footstepTimer <= 0)
    //         {
    //             PlayFootstepSound();
    //             footstepTimer = footstepInterval; // Reset timer
    //         }
    //     }
    //     else
    //     {
    //         footstepTimer = 0; // Reset timer when not moving
    //     }
    // }

    void PlayFootstepSound()
    {
        if (isGrounded) // Ensure the player is grounded before playing the sound
        {
            if (!audioSource.isPlaying) // Only play the sound if it's not already playing
            {
                if (audioSource != null && footstepSound != null)
                {
                    audioSource.PlayOneShot(footstepSound); // Play footstep sound only when grounded
                }
                else
                {
                    Debug.LogWarning("Audio source or footstep sound not assigned");
                }
            }
        }
    }
   
}
