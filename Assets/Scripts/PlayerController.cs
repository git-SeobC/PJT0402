using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerHurt hurt;
    private PlayerDie die;
    private Rigidbody2D rb;

    private Vector3 StartPlayerPos;

    private bool isPaused = false;
    public GameObject pauseMenuUI;

    [Header("카메라 쉐이크 설정")]
    public CinemachineImpulseSource impulseSource;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.0f;
    public float knockbackForce = 5.0f;
    private bool isKnockback = false;
    public float knockbackDuration = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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

        if (isPaused || isKnockback) return;

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

    public void GenerateCameraImpulse()
    {
        if (impulseSource != null)
        {
            Debug.Log("카메라 임펄스 발생");
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.Log("NO Impulse");
        }
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
            GenerateCameraImpulse();

            if (!isInvincible)
            {
                SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
                StartCoroutine(Invincibility());
                animator.SetBool("IsFalling", false);
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
        Time.timeScale = 0.8f; // 맞았을 경우 살짝 시간이 느리게 가도록하는 디테일
        // 플레이어 속도를 살짝 높여 위험에서 빠르게 벗어날 수 있도록 하는 것도 좋음

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
        Time.timeScale = 1.0f;
        isInvincible = false;
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }
}
