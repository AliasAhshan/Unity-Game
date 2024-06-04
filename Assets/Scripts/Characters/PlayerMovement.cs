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
    public ParticleSystem deathParticles;
    public AudioClip deathSound;
    public AudioSource audioSource;

    bool dead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        Debug.Log("Player started with health: " + currentHealth);
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
            Debug.Log("Player jumped");
        }

        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }

        if (dead && Input.GetButtonDown("Jump"))
        {
            ReloadScene();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
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

    private IEnumerator Die()
    {
        dead = true;
        Debug.Log("Player is dead");
        
        // Ensure death particles and sound play
        if (deathParticles != null)
        {
            deathParticles.Emit(10);
            Debug.Log("Death particles emitted");
        }
        else
        {
            Debug.LogWarning("Death particles not assigned");
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            Debug.Log("Death sound played");
        }
        else
        {
            Debug.LogWarning("Audio source or death sound not assigned");
        }

        // Disable player control
        horizontalInput = 0;
        rb.velocity = Vector2.zero;
        enabled = false; // Disable the PlayerMovement script

        yield return new WaitForSeconds(2f);

        Debug.Log("Reloading scene");
        ReloadScene();
    }

    void ReloadScene()
    {
        Debug.Log("Scene reloading");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
