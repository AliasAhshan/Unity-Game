using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public LayerMask wallLayer;
    private Animator animator;
    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;  // Radius for ground check
    private float speed = 5f;
    private float jumpPower = 10f;
    private bool isJumping;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        //bool isGrounded = IsGrounded();

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        if (/*!isGrounded &&*/ !isJumping) {
            // This ensures we only set IsJumping when initially leaving the ground
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        if (/*isGrounded &&*/ isJumping) {
            // This ensures we reset jumping only after being airborne
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }

        // Move the player
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        // Jumping logic
        if (Input.GetButtonDown("Jump") /*&& isGrounded*/)
        {
            rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        if (animator.applyRootMotion) {
        Debug.Log("Root motion is applied, which might be affecting movement.");
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 direction = horizontalInput > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.fixedDeltaTime, wallLayer);

        if (!hit.collider)
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement if a wall is detected
        }
    }

    //private bool IsGrounded()
    //{
        //return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    //}
}
