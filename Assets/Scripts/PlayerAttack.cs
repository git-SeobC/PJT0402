using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private Animator animator;

    public bool isAttacking = false;

    [Header("애니메이션 상태 이름")]
    public string attackStateName = "Attack";

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
    }

    public void PerformAttack()
    {
        if (isAttacking) // 공격 상태 체크
        {
            return;
        }

        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
        }
        StartCoroutine(AttackCooldownByAnimation());
    }

    private IEnumerator AttackCooldownByAnimation()
    {
        isAttacking = true;
        SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
        yield return null; // 다음 프레임까지의 안정성을 위해 추가
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(attackStateName))
        {
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isAttacking = false;
    }
}
