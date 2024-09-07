using UnityEngine;
using System.Collections;

public class CentipedeEnemy : MonoBehaviour
{
    public Transform player;             // Reference to the player
    public float moveSpeed = 5f;         // Movement speed of the centipede
    public float attackRange = 1.5f;     // Range at which the centipede should attack
    public float attackCooldown = 2f;    // Time between consecutive attacks
    private float lastAttackTime = 0f;   // Time since the last attack
    public float attackDelay = 0.5f;     // Time delay before applying damage (for animation)

    private Animator animator;           // Reference to the Animator
    private bool isAttacking = false;    // Flag to check if centipede is attacking
    public LayerMask groundLayer;        // Layer for ground detection
    public int attackDamage = 10;

    private Rigidbody2D rb;

    public float raycastDistance = 1.5f; // Distance for ground detection raycast
    public float rotationSpeed = 5f;     // Speed at which centipede rotates to match the ground slope

    // Add more raycast points across the centipede's body
    public Transform[] bodyRaycastPoints; // Array of transforms to hold raycast points along the body

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
        rb = GetComponent<Rigidbody2D>();     // Reference to the Rigidbody2D for movement and physics

        // Load the saved centipede position when the game starts
        LoadCentipedePosition();
    }

    void Update()
    {
        // Ground check and adjust rotation based on multiple raycasts across the body
        AdjustRotationToGround();
        ApplyGravityFromRaycasts();

        // Check distance to the player for attacking
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Follow the player continuously, no matter the distance
        if (!isAttacking)
        {
            MoveTowardsPlayer();
        }

        // If within attack range and cooldown is over, start the attack
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(StartAttackWithDelay());
        }
    }

    void AdjustRotationToGround()
    {
        Vector2 averageNormal = Vector2.zero; // To store the average normal
        int raycastHits = 0; // To count how many raycasts hit the ground

        // Loop through all body raycast points and perform raycasts
        foreach (Transform raycastPoint in bodyRaycastPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, Vector2.down, raycastDistance, groundLayer);
            Debug.DrawRay(raycastPoint.position, Vector2.down * raycastDistance, Color.green); // Visualize the raycast

            // If the raycast hits the ground, add its normal to the average
            if (hit.collider != null)
            {
                averageNormal += hit.normal;
                raycastHits++;
            }
        }

        if (raycastHits > 0)
        {
            // Calculate the average normal
            averageNormal /= raycastHits;

            // Calculate the slope angle based on the average normal
            float slopeAngle = Mathf.Atan2(averageNormal.y, averageNormal.x) * Mathf.Rad2Deg;

            // Rotate the centipede to align with the slope
            Quaternion targetRotation = Quaternion.Euler(0, 0, slopeAngle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void ApplyGravityFromRaycasts()
    {
        bool anyGroundHit = false; // Track if any raycast hits the ground

        // Loop through all raycast points and perform raycasts
        foreach (Transform raycastPoint in bodyRaycastPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, Vector2.down, raycastDistance, groundLayer);
            Debug.DrawRay(raycastPoint.position, Vector2.down * raycastDistance, Color.green); // Visualize the raycast

            // If the raycast hits the ground, the centipede is grounded at that point
            if (hit.collider != null)
            {
                anyGroundHit = true; // At least one raycast hits the ground
            }
        }

        // If none of the raycasts hit the ground, apply downward force (gravity)
        if (!anyGroundHit)
        {
            rb.AddForce(Vector2.down * 100f);  // Adjust this value for the desired gravity effect
        }
    }


    void MoveTowardsPlayer()
    {
        if (!isAttacking)  // Only move if not attacking
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
        }
    }

    IEnumerator StartAttackWithDelay()
    {
        isAttacking = true;  // Mark the centipede as attacking
        animator.SetBool("isAttacking", true);  // Trigger attack animation

        // Wait for the attack delay to allow the animation to play
        yield return new WaitForSeconds(attackDelay);

        // Inflict damage after the delay
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            Debug.Log("Centipede attacks the player.");
            playerMovement.TakeDamage(attackDamage);  // Inflict damage to the player
        }

        lastAttackTime = Time.time;  // Reset cooldown timer

        // End attack after cooldown
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;  // Stop attacking
        animator.SetBool("isAttacking", false);  // Reset to walking animation
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger attack if player enters the range
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StartAttackWithDelay());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Stop attacking when the player leaves the range
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }

    // Save the centipede's position
    public void SaveCentipedePosition()
    {
        Vector2 centipedePosition = new Vector2(transform.position.x, transform.position.y);
        SaveSystem.SaveGame(player.position, centipedePosition, player.GetComponent<PlayerMovement>().currentHealth);
    }

    // Load the centipede's position
    public void LoadCentipedePosition()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            transform.position = new Vector3(data.centipedePosition.x, data.centipedePosition.y, transform.position.z);
        }
    }
}
