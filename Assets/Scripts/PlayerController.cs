using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerHurt hurt;
    private PlayerDie die;
    private Rigidbody2D rb;

    private Vector3 StartPlayerPos;

    private bool isPaused = false;
    public GameObject pauseMenuUI;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.0f;
    public float knockbackForce = 5.0f;
    private bool isKnockback = false;
    public float knockbackDuration = 0.2f;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        hurt = GetComponent<PlayerHurt>();
        die = GetComponent<PlayerDie>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartPlayerPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ReGame();
            }
            else
            {
                Pause();
            }
        }

        if (isPaused) return;

        if (Input.GetButtonDown("Fire1") && !gameObject.GetComponent<PlayerMovement>().isFalling)
        {
            if (gameObject.GetComponent<PlayerMovement>().isGrounded) rb.linearVelocity = new Vector2(0, 0);
            attack.PerformAttack();
        }
        else if (attack.isAttacking == false && isKnockback == false)
        {
            movement.HandleMovement();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        SoundManager.Instance.PlaySFX(SFXType.MenuOpenSFX);
    }

    public void ReGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
        SoundManager.Instance.PlaySFX(SFXType.MenuOpenSFX);
    }

    public void ResumeButtonClick()
    {
        ReGame();
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemApple"))
        {
            GameManager.Instance.AddCoin(1);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("DeathZone"))
        {
            SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
            transform.position = StartPlayerPos;
        }
        else if (collision.CompareTag("Trap"))
        {
            PlayerAttack playerAttack = GetComponent<PlayerAttack>();
            float shakeDuration = 0.1f;
            float shakeMagnitude = 0.3f;
            //StartCoroutine(playerAttack.Shake(shakeDuration, shakeMagitude))
            // 카메라 CinemachineImpulseSource로 임펄스 쉐이크 진행하도록 변경해야함

            if (!isInvincible)
            {
                SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
                StartCoroutine(Invincibility());
                animator.SetTrigger("Hit");
                Vector2 knockbackDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                StartCoroutine(KnockbackCoroutine());
            }
        }
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;

        float elapsedTime = 0f;
        float blinkInterval = 0.2f;

        Color originalColor = spriteRenderer.color;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval * 2;
        }
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }
}
