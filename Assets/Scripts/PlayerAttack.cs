using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private Animator animator;
    public List<GameObject> attackObjList = new List<GameObject>();
    public bool isAttacking = false;

    [Header("애니메이션 상태 이름")]
    public string attackStateName = "Attack";

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    private Vector3 originalPos;

    [Header("카메라 쉐이크 설정")]
    public CinemachineImpulseSource impulseSource;

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();

        if (Camera.main != null)
        {
            originalPos = Camera.main.transform.localPosition;
        }
    }

    public void PerformAttack()
    {
        if (isAttacking) // 공격 상태 체크
        {
            return;
        }


        if (playerAnimation != null)
        {
            playerAnimation.SetAttack(true);
            playerAnimation.TriggerAttack();
        }
        StartCoroutine(AttackCooldownByAnimation());
    }

    private IEnumerator AttackCooldownByAnimation()
    {
        isAttacking = true;
        originalPos = Camera.main.transform.localPosition;
        //StartCoroutine(Shake(shakeDuration, shakeMagnitude));
        GenerateCameraImpulse();
        SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
        //ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, transform.position + new Vector3(1, 1, 0), new Vector3(5, 5, 5));
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
        playerAnimation.SetAttack(false);
        AttackEnd();
    }

    /// <summary>
    /// 애니메이션에서 호출하는 메소드
    /// </summary>
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

    /// <summary>
    /// 애니메이션에서 호출하는 메소드
    /// </summary>
    public void AttackEnd()
    {
        attackObjList[0].SetActive(false);
        attackObjList[1].SetActive(false);
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        if (Camera.main == null)
        {
            yield break;
        }

        var cinema = Camera.main.GetComponent<CinemachineBrain>();

        cinema.enabled = false;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x, originalPos.y + y, -10);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;

        cinema.enabled = true;
    }

    private void GenerateCameraImpulse()
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

}
