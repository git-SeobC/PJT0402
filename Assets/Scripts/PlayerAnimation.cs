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
        animator.SetTrigger("Land");
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
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
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", isFalling);
    }
}
