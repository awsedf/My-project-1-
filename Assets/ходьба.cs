using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Wall Slide & Jump")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForceX = 10f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallJumpDuration = 0.2f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 0.8f);

    [Header("Ground & Wall Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isRunning;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isFacingRight = true;
    private float currentSpeed;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpTime;
    private bool isTouchingWall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleInput();
        UpdateTimers();
        CheckWallSlide();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJumpPhysics();
    }

    private void HandleInput()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;

            if (isWallSliding)
            {
                WallJump();
            }
        }
    }

    private void UpdateTimers()
    {
        coyoteTimeCounter = IsGrounded() ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        if (isWallJumping)
        {
            wallJumpTime -= Time.deltaTime;
            if (wallJumpTime <= 0)
                isWallJumping = false;
        }
    }

    private void CheckWallSlide()
    {
        Vector2 checkPos = wallCheck.position;
        isTouchingWall = Physics2D.OverlapBox(
            checkPos + (isFacingRight ? Vector2.right : Vector2.left) * wallCheckDistance,
            wallCheckSize,
            0f,
            groundLayer
        );

        isWallSliding = isTouchingWall && !IsGrounded() && rb.velocity.y < 0;
    }

    private void HandleMovement()
    {
        if (isWallJumping) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        float targetSpeed = inputX * (isRunning ? runSpeed : walkSpeed);

        if (inputX != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
            
            if (!isWallSliding && ((inputX > 0 && !isFacingRight) || (inputX < 0 && isFacingRight)))
                Flip();
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }
    }

    private void HandleJumpPhysics()
    {
        if (isWallSliding) return;

        if (rb.velocity.y < 0)
            rb.gravityScale = fallMultiplier;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 1f;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void WallJump()
    {
        if (!isTouchingWall) return; // Ключевая проверка!

        isWallJumping = true;
        wallJumpTime = wallJumpDuration;

        float direction = isFacingRight ? -1f : 1f;
        rb.velocity = new Vector2(direction * wallJumpForceX, wallJumpForceY);
        Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация проверки земли
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Визуализация проверки стены
        Gizmos.color = Color.blue;
        Vector2 checkPos = wallCheck.position;
        Gizmos.DrawWireCube(
            checkPos + (isFacingRight ? Vector2.right : Vector2.left) * wallCheckDistance,
            wallCheckSize
        );
    }
}
