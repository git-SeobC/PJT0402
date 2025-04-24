using NUnit.Framework.Internal.Commands;
using System.Collections;
using Unity.AppUI.Core;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDie die;
    private Rigidbody2D rb;

    private bool isPaused = false;
    public GameObject pauseMenuUI;

    [Header("카메라 쉐이크 설정")]
    public CinemachineImpulseSource impulseSource;

    private CapsuleCollider2D cld;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.0f;
    public float knockbackForce = 5.0f;
    private bool isKnockback = false;
    public float knockbackDuration = 0.2f;

    public int playerHP = 3;
    private int maxPlayerHP = 3;

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
        die = GetComponent<PlayerDie>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cld = GetComponent<CapsuleCollider2D>();
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

        if (isPaused || isKnockback || (GameManager.Instance?.changeMap ?? false)) return;

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
        if (collision.CompareTag("Life"))
        {
            if (playerHP < maxPlayerHP)
            {
                playerHP++;
                GameManager.Instance.SetLifeUI();
            }
            SoundManager.Instance.PlaySFX(SFXType.ItemLifeSFX);
            ParticleManager.Instance.ParticlePlay(ParticleType.ItemGet, collision.transform.position);
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("DeathZone"))
        {
            playerHP = 0;
            GameManager.Instance.SetLifeUI();
            if (playerHP <= 0)
            {
                die.Die(collision);
            }
            //SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
            //transform.position = StartPlayerPos;
        }
        else if (collision.CompareTag("Trap") || collision.CompareTag("EnemyAttack"))
        {
            GenerateCameraImpulse();

            if (!isInvincible)
            {
                playerHP--;
                GameManager.Instance.SetLifeUI();
                if (playerHP <= 0)
                {
                    die.Die(collision);
                }
                else
                {
                    SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
                    ParticleManager.Instance.ParticlePlay(ParticleType.PlayerDamage, transform.position, new Vector3(4, 4, 4));
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
    }

    /// <summary>
    /// 피격시 무적 상태
    /// </summary>
    /// <returns></returns>
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
        cld.enabled = false;
        cld.enabled = true;
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }
}
