using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("플레이어 능력치")]
    public int maxHP = 100;
    public int currentHP;
    public int damage = 10;
    public float attackSpeed = 1.0f;
    public float moveSpeed = 3.0f;

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
        currentHP = maxHP;
    }


    public void TakeDamage(int amount)
    {
        SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);

        currentHP -= amount;
        if (currentHP <= 0)
        {
            //Die
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    public void Die()
    {

    }

    public void UpgradeDamage()
    {

    }



    void Start()
    {

    }

    void Update()
    {

    }
}
