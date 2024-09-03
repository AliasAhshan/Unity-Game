using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed of the player
    public float jumpPower = 5f; // Jump power of the player
    public float slideSpeed = 8f; // Speed during slide
    public float slideDuration = 0.5f; // Duration of the slide

    public GameObject saveGamePopup;

    bool isSliding = false; // To track if the player is currently sliding

    BoxCollider2D boxCollider; // To reference the player's collider
    Vector2 originalColliderSize; // Store the original size of the collider
    Vector2 originalColliderOffset; // Store the original offset of the collider


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
    public bool canMove = true;

    void Start()
    {
        //SaveSystem.DeleteSave();

        // Optionally reload the scene
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Store the original collider size and offset
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;

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

        if (SaveSystem.LoadGame() != null)
        {
            LoadGame();
        }

    }

    void Update()
    {
        if (!canMove) return;

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

        // Handle sliding input (trigger on key press, not hold)
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            StartCoroutine(Slide());
        }

        // Handle jumping animation
        if (isGrounded && rb.velocity.y <= 0 && !isSliding)
        {
            animator.SetBool("isJumping", false);
        }
        else if (!isGrounded && rb.velocity.y > 0 && !isSliding)
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
        HandleFootstepSound();

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
        if (!canMove) return;

        // Check if the player is pushing against a wall
        bool isPushingAgainstWall = IsPushingAgainstWall();

        // Check if the player is pushing against the ground
        bool isPushingAgainstGround = IsPushingAgainstGround();

        // Determine if the character is moving away from the wall or ground
        bool isMovingAwayFromObstacle = (horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight);

        if (animator.GetBool("isSliding"))
        {
            // Sliding is handled in the Slide() coroutine
            return; // Skip other movement code while sliding
        }


        // Handle horizontal movement
        if ((isPushingAgainstWall || isPushingAgainstGround) && Mathf.Abs(rb.velocity.y) > 0 && !isMovingAwayFromObstacle)
        {
            // Player is pushing against a wall or ground and not moving away, prevent horizontal movement to allow sliding
            rb.velocity = new Vector2(0, rb.velocity.y); // Set horizontal velocity to 0 to prevent sticking
        }
        else if (isMovingAwayFromObstacle)
        {
            // If moving away from the obstacle, ensure normal horizontal movement
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Normal horizontal movement when not pushing against anything
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

        // Stop the footstep sound if the player is not moving horizontally
        if (Mathf.Abs(horizontalInput) == 0 && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void ShowSaveGamePopup()
    {
        Debug.Log("Showing Save Game Popup");
        canMove = false;  // Disable player movement
        saveGamePopup.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Debug.Log("Game Paused");
    }

    public void OnSaveGameYes()
    {
        Debug.Log("Button Clicked");
        Debug.Log("Saving Game");
        SaveGame();
        saveGamePopup.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        canMove = true; // Re-enable player movement
        Debug.Log("Game Resumed");
    }

    public void OnSaveGameNo()
    {
        Debug.Log("Button Clicked");
        Debug.Log("Cancel Saving Game");
        saveGamePopup.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        canMove = true; // Re-enable player movement
    }

    void SaveGame()
    {
        Vector2 playerPosition = transform.position;
        int health = currentHealth;
        SaveSystem.SaveGame(playerPosition, health);
    }

    void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            Vector2 adjustedPosition = data.playerPosition;
            adjustedPosition.x += 2.0f; // Adjust this value as needed to place the player just outside the save zone
            transform.position = adjustedPosition;
            currentHealth = data.currentHealth;

            Debug.Log("Game loaded, player position: " + data.playerPosition);
            Debug.Log("Game loaded, player health: " + data.currentHealth);

            // Ensure the camera or any other systems are updated with the loaded state
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
    }


    void RestartGame()
    {
        SaveSystem.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);

        // Reduce collider size during slide
        boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 6);
        boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - (originalColliderSize.y - boxCollider.size.y) / 2); // Adjust offset to prevent dipping

        // Calculate the direction of the slide based on the character's facing direction
        float slideDirection = isFacingRight ? 1f : -1f;

        // Apply forward movement without downward force
        rb.velocity = new Vector2(slideDirection * moveSpeed * 1.5f, rb.velocity.y); // No vertical change

        // Wait for the duration of the slide
        yield return new WaitForSeconds(slideDuration);

        // Restore collider size after slide
        boxCollider.size = originalColliderSize;
        boxCollider.offset = originalColliderOffset;

        isSliding = false;
        animator.SetBool("isSliding", false);

        // Ensure the player is grounded after sliding
        yield return new WaitForFixedUpdate();
        if (isGroundedCheck())
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }



    private bool IsPushingAgainstWall()
    {
        // Define offsets for multiple raycasts at different heights on the body
        float[] raycastOffsets = { 0.1f, 0.5f, 1f }; // Adjust these values based on your character's height

        bool wallDetected = false;

        foreach (float offset in raycastOffsets)
        {
            // Get the starting points for the raycasts (lower on the character)
            Vector3 rayStartLeft = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
            Vector3 rayStartRight = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);

            // Cast rays horizontally from the lower positions to check for walls
            RaycastHit2D wallCheckLeft = Physics2D.Raycast(rayStartLeft, Vector2.left, 0.5f, LayerMask.GetMask("Platform"));
            RaycastHit2D wallCheckRight = Physics2D.Raycast(rayStartRight, Vector2.right, 0.5f, LayerMask.GetMask("Platform"));

            Debug.DrawRay(rayStartLeft, Vector2.left * 0.5f, Color.blue); // Visualize the left raycast in Scene view
            Debug.DrawRay(rayStartRight, Vector2.right * 0.5f, Color.blue); // Visualize the right raycast in Scene view

            // Use surface normal to detect walls (vertical surfaces)
            bool leftWall = wallCheckLeft.collider != null && Mathf.Abs(wallCheckLeft.normal.x) > 0.7f;
            bool rightWall = wallCheckRight.collider != null && Mathf.Abs(wallCheckRight.normal.x) > 0.7f;

            if (leftWall || rightWall)
            {
                wallDetected = true;
                break; // Exit the loop early if a wall is detected at any height
            }
        }

        return wallDetected; // Return true if any ray detects a wall
    }

    private bool IsPushingAgainstGround()
    {
        // Adjust the offset to cast the ray from a lower or higher point on the body
        float raycastOffset = 0.7f; // Adjust this value to change where the ray starts on the body

        // Get the starting points for the raycasts (lower on the character)
        Vector3 rayStartLeft = new Vector3(transform.position.x, transform.position.y - raycastOffset, transform.position.z);
        Vector3 rayStartRight = new Vector3(transform.position.x, transform.position.y - raycastOffset, transform.position.z);

        // Cast rays horizontally from the lower positions to check for walls
        RaycastHit2D wallCheckLeft = Physics2D.Raycast(rayStartLeft, Vector2.left, 0.3f, LayerMask.GetMask("Ground"));
        RaycastHit2D wallCheckRight = Physics2D.Raycast(rayStartRight, Vector2.right, 0.3f, LayerMask.GetMask("Ground"));

        Debug.DrawRay(rayStartLeft, Vector2.left * 0.5f, Color.blue); // Visualize the left raycast in Scene view
        Debug.DrawRay(rayStartRight, Vector2.right * 0.5f, Color.blue); // Visualize the right raycast in Scene view

        // Use surface normal to detect walls (vertical surfaces)
        bool leftWall = wallCheckLeft.collider != null && Mathf.Abs(wallCheckLeft.normal.x) > 0.7f;
        bool rightWall = wallCheckRight.collider != null && Mathf.Abs(wallCheckRight.normal.x) > 0.7f;

        return leftWall || rightWall; // Return true if either wall is detected
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
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayLength, LayerMask.GetMask("Ground", "Platform"));

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


        Debug.Log("Entered Trigger Zone with: " + collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("SavePoint"))
        {
            collision.GetComponent<Collider2D>().enabled = false;
            ShowSaveGamePopup();
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
                deathParticles.Emit(70);  // Emit 10 particles
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

    void HandleFootstepSound()
    {
        // Play footstep sound if grounded and moving
        if (Mathf.Abs(horizontalInput) > 0 && isGrounded)
        {
            PlayFootstepSound();
        }
        // else if (audioSource.isPlaying)
        // {
        //     audioSource.Stop(); // Stop the footstep sound if the player stops moving
        // }
    }

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
