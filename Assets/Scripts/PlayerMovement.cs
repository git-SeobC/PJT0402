using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFalling;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private PlayerAnimation playerAnimation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // velocity = 

        if (playerAnimation != null)
        {
            playerAnimation.SetWalking(moveInput != 0);
        }

        if (moveInput != 0)
        {
            //Debug.Log("이동 입력 : " + moveInput);
            GetComponent<SpriteRenderer>().flipX = moveInput < 0;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded) // 땅에 붙어있는지 확인
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //playerAnimation.SetJumping(true);
            playerAnimation.JumpStart();
            Debug.Log("점프");
        }
        else if (!isGrounded && rb.linearVelocity.y < -0.1f) // 낙하 상태
        {
            isFalling = true;
            playerAnimation.SetFalling(true);
        }
        else if (isGrounded && isFalling) // 착지 상태
        {
            isFalling = false;
            playerAnimation?.PlayerLanding();
        }

        //bool isAirborne = !isGrounded;
        //playerAnimation.SetJumping(isAirborne);

    }
}
