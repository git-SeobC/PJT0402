using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerHurt hurt;
    private PlayerDie die;

    private Vector3 StartPlayerPos;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        hurt = GetComponent<PlayerHurt>();
        die = GetComponent<PlayerDie>();
    }

    void Start()
    {
        StartPlayerPos = transform.position;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("buttondown fire1");
            attack.PerformAttack();
        }
        else if (attack.isAttacking == false)
        {
            movement.HandleMovement();
        }
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
