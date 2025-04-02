using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerHurt hurt;
    private PlayerDie die;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        hurt = GetComponent<PlayerHurt>();
        die = GetComponent<PlayerDie>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        movement.HandleMovement();

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("buttondown fire1");
            attack.PerformAttack();
        }
    }
}
