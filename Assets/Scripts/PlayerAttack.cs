using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private Animator animator;
    public List<GameObject> attackObjList = new List<GameObject>();
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
        ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, transform.position + new Vector3(1, 1, 0), new Vector3(5, 5, 5));
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

    public void AttackStart()
    {
        bool isFacingLeft = GetComponent<SpriteRenderer>().flipX;
        if (isFacingLeft)
        {
            if (attackObjList.Count > 0)
            {
                attackObjList[0].SetActive(true);
            }
        }
        else
        {
            if (attackObjList.Count > 0)
            {
                attackObjList[1].SetActive(true);
            }
        }
    }

    public void AttackEnd()
    {
        bool isFacingLeft = GetComponent<SpriteRenderer>().flipX;
        if (isFacingLeft)
        {
            if (attackObjList.Count > 0)
            {
                attackObjList[0].SetActive(false);
            }
        }
        else
        {
            if (attackObjList.Count > 0)
            {
                attackObjList[1].SetActive(false);
            }
        }
    }

}
