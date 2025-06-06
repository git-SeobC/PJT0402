using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;

    public Rigidbody2D rb;
    public bool isGrounded;
    public bool isFalling;

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
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (playerAnimation != null)
        {
            playerAnimation.SetWalking(moveInput != 0);
        }

        if (moveInput != 0)
        {
            GetComponent<SpriteRenderer>().flipX = moveInput < 0;
        }

        // 스케일을 통한 좌우 반전
        //if (moveInput < 0 && transform.localScale.x > 0)
        //{
        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //}
        //else if (moveInput > 0 && transform.localScale.x < 0)
        //{
        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //}
        int combinedLayers = LayerMask.GetMask("Ground", "EnemyHead");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, combinedLayers);
        if (Input.GetButtonDown("Jump") && isGrounded) // 땅에 붙어있는지 확인
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //playerAnimation.SetJumping(true);
            playerAnimation.JumpStart();
            SoundManager.Instance.PlaySFX(SFXType.PlayerJumpSFX);
        }
        else if (!isGrounded && rb.linearVelocity.y < 0) // 낙하 상태
        {
            isFalling = true;
            playerAnimation.SetFalling(true);
        }
        else if (isGrounded && isFalling) // 착지 상태
        {
            isFalling = false;
            playerAnimation.PlayerLanding();
        }

        //bool isAirborne = !isGrounded;
        //playerAnimation.SetJumping(isAirborne);
    }

    public void StepSoundEvent()
    {
        SoundManager.Instance.PlaySFX(SFXType.PlayerStepSFX);
    }
}
