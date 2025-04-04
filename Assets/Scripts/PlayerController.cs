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

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        hurt = GetComponent<PlayerHurt>();
        die = GetComponent<PlayerDie>();
        rb = GetComponent<Rigidbody2D>();
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
        else if (attack.isAttacking == false)
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
    }
}
