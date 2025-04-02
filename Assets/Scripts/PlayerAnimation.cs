using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAttack()
    {
        Debug.Log("Attack Trigger On");
        animator.SetTrigger("Attack");
    }
    public void SetWalking(bool isWalking)
    {
        animator.SetBool("IsWalking", isWalking);
    }

    public void PlayerJumpAnimation()
    {
        animator.SetTrigger("IsJump");
    }

    public void PlayerLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetTrigger("Land");
    }

    public void JumpStart()
    {
        animator.SetTrigger("JumpStart");
        animator.SetBool("IsJumping", true);
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetFalling(bool isFalling)
    {
        animator.SetBool("IsJumpint", false);
        animator.SetBool("IsFalling", isFalling);
    }
}
