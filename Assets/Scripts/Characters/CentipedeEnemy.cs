using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CentipedeEnemy : MonoBehaviour
{
    public Transform player;             // Reference to the player
    public float moveSpeed = 5f;         // Base movement speed of the centipede
    public float minMoveSpeed = 1f;      // Minimum movement speed when near the player
    public float decelerationRange = 3f; // Distance from the player where deceleration starts
    public float attackRange = 1.5f;     // Range at which the centipede should attack
    public float attackCooldown = 2f;    // Time between consecutive attacks
    private float lastAttackTime = 0f;   // Time since the last attack
    public float attackDelay = 0.5f;     // Time delay before applying damage (for animation)
    public float raycastDistance = 1.5f; // Distance for ground detection raycast
    public float rotationSpeed = 5f;     // Speed at which centipede rotates to match the ground slope

    private Animator animator;           // Reference to the Animator
    private bool isAttacking = false;    // Flag to check if centipede is attacking
    public LayerMask groundLayer;        // Layer for ground detection
    public int attackDamage = 10;

    private NavMeshAgent agent;          // Reference to the NavMeshAgent

    // Add more raycast points across the centipede's body
    public Transform[] bodyRaycastPoints; // Array of transforms to hold raycast points along the body

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent

        // Set up NavMeshAgent for 2D use
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
        agent.baseOffset = 1f;

        // Disable Rigidbody2D gravity, let NavMeshAgent handle movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics on Rigidbody2D
        }

        // Load the saved centipede position when the game starts
        LoadCentipedePosition();
    }

    void Update()
    {
        // Keep Z position constant (2D plane)
        Vector3 position = transform.position;
        position.z = 0;  // Set Z to 0 to keep the centipede on the 2D plane
        transform.position = position;

        // Calculate the distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Decelerate when approaching the player within the deceleration range
        if (distanceToPlayer <= decelerationRange)
        {
            // Calculate speed reduction based on proximity to the player
            float adjustedSpeed = Mathf.Lerp(minMoveSpeed, moveSpeed, distanceToPlayer / decelerationRange);
            agent.speed = adjustedSpeed;
        }
        else
        {
            // Set the speed to normal when outside deceleration range
            agent.speed = moveSpeed;
        }

        // Set the destination for the NavMeshAgent
        agent.SetDestination(player.position);

        // Adjust rotation to match ground slope
        AdjustRotationToGround();

        // Check if the centipede should attack
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
