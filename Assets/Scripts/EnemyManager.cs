using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    None, Orc
}

public enum StateType
{
    None,
    Idle,
    Patrol,
    Chase,
    Attack,
    Hit,
    Death,
}

public class EnemyManager : MonoBehaviour
{
    private Color orginalColor;
    private Renderer objectRenderer;
    public float colorChangeDuration = 0.5f;
    public float enemyHp = 10.0f;
    public float speed = 2.0f;
    public float damage = 1.0f;
    public float maxDistance = 3.0f;
    private Vector3 startPos;
    private int direction = 1;
    public EnemyType enemyType = EnemyType.None;
    public StateType currentState = StateType.Idle;

    public Transform player;
    public float chaseRange = 3.0f;
    public float attackRange = 1.0f;
    public bool isAttacking = false;

    //private float stateChangeInterval = 3.0f;
    private Coroutine stateChangeRoutine;

    public bool isGrounded;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Transform[] patrolPoints;
    private int currentPoint = 0;
    private Animator animator;
    private float distanceToPlayer;
    public bool isInvincible = false;
    public float invincibilityDuration = 0.5f;
    public float knockbackForce = 5.0f;
    //private bool isKnockback = false;
    public float knockbackDuration = 0.2f;
    private CapsuleCollider2D cld;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public List<GameObject> attackObjList = new List<GameObject>();

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        orginalColor = objectRenderer.material.color;
        startPos = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cld = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (enemyType != EnemyType.None && player != null) ChangeState(StateType.Idle);
    }

    private void Update()
    {
        if (enemyType == EnemyType.None || player == null) return;

        if (enemyType == EnemyType.Orc)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            distanceToPlayer = Vector2.Distance(transform.position, player.position);
            #region comment

            //if (enemyHp <= 0) currentState = StateType.Death;

            //if (currentState == StateType.Hit)
            //{
            //    if (isInvincible == true) return;
            //    StopAllCoroutines();
            //    SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
            //    ParticleManager.Instance.ParticlePlay(ParticleType.PlayerDamage, transform.position, new Vector3(4, 4, 4));
            //    StartCoroutine(Invincibility());
            //    Vector2 knockbackDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
            //    animator.SetTrigger("Hit");
            //    rb.linearVelocity = Vector2.zero;
            //    rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            //    StartCoroutine(KnockbackCoroutine());
            //    return;
            //}
            //else if (currentState == StateType.Death)
            //{

            //}

            //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            //distanceToPlayer = Vector2.Distance(transform.position, player.position);

            //if (distanceToPlayer <= attackRange && isAttacking == false)
            //{
            //    if (currentState != StateType.Attack)
            //    {
            //        StopAllCoroutines();
            //        currentState = StateType.Attack;
            //        StartCoroutine(AttackRoutine());
            //    }
            //    return;
            //}

            //if (distanceToPlayer <= chaseRange && isAttacking == false)
            //{
            //    if (currentState != StateType.Chase)
            //    {
            //        if (stateChangeRoutine != null)
            //        {
            //            StopCoroutine(stateChangeRoutine);
            //        }
            //        //Debug.Log($"[상태 전환] 추적 상태 : {stateType}");
            //    }
            //    Vector3 directionToPlayer = (player.position - transform.position).normalized;
            //    if (directionToPlayer.x > 0)
            //    {
            //        GetComponent<SpriteRenderer>().flipX = false;
            //    }
            //    else
            //    {
            //        GetComponent<SpriteRenderer>().flipX = true;
            //    }

            //    animator.SetBool("IsWalk", true);
            //    transform.position += directionToPlayer * speed * Time.deltaTime;
            //    return;
            //}

            ////if (currentState == StateType.Chase && distanceToPlayer > chaseRange * 2)
            ////{
            ////    //Debug.Log("[상태 전환] 추적 종료");
            ////    currentState = StateType.Patrol;
            ////}

            //if (currentState == StateType.Attack) return;
            //PatrolMovement(); 
            #endregion
        }
    }

    IEnumerator KnockbackCoroutine()
    {
        //isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        //isKnockback = false;
    }

    public void ChangeState(StateType newState, float Param = 5.0f)
    {
        if (currentState == newState) return;
        if (stateChangeRoutine != null)
        {
            StopCoroutine(stateChangeRoutine);
            //StopAllCoroutines();
        }

        currentState = newState;
        //animator.SetBool("IsWalk", false);

        switch (currentState)
        {
            case StateType.Idle:
                stateChangeRoutine = StartCoroutine(Idle());
                break;
            case StateType.Patrol:
                stateChangeRoutine = StartCoroutine(Patrol());
                break;
            case StateType.Chase:
                stateChangeRoutine = StartCoroutine(Chase());
                break;
            case StateType.Attack:
                stateChangeRoutine = StartCoroutine(Attack());
                break;
            case StateType.Death:
                stateChangeRoutine = StartCoroutine(Death());
                break;
            case StateType.None:
                StopAllCoroutines();
                break;

        }
    }

    private IEnumerator Death()
    {
        animator.SetTrigger("Death");
        SoundManager.Instance.PlaySFX(SFXType.OrcDeathSFX);
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
        ChangeState(StateType.None);
    }

    public void Hit(float damage, string pCollisionName = "")
    {
        animator.SetBool("IsWalk", false);
        StartCoroutine(HitCoroutine(damage, pCollisionName));
    }
    private IEnumerator HitCoroutine(float pDamage = 5.0f, string pCollisionName = "")
    {
        if (isInvincible) yield break;
        isInvincible = true;
        //StopAllCoroutines();
        enemyHp -= pDamage;
        SoundManager.Instance.PlaySFX(SFXType.OrcHitSFX);

        if (enemyHp > 0)
        {
            animator.SetTrigger("Hit");
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                yield return null;
            }

            // 넉백 방향 확인
            Vector2 knockbackDirection;
            if (pCollisionName == "AttackObjRight") knockbackDirection = Vector2.right;
            else if (pCollisionName == "AttackObjLeft") knockbackDirection = Vector2.left;
            else knockbackDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            float elapsedTime = 0f;
            float blinkInterval = 0.2f;
            Color originalColor = spriteRenderer.color;
            while (elapsedTime < invincibilityDuration ||
                (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);
                yield return new WaitForSeconds(blinkInterval);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
                yield return new WaitForSeconds(blinkInterval);
                elapsedTime += blinkInterval * 2;
            }
            cld.enabled = false;
            cld.enabled = true;
            isInvincible = false;

            //StartCoroutine(Invincibility());
            ChangeState(StateType.Chase);
        }
        else
        {
            ChangeState(StateType.Death);
        }
    }

    private IEnumerator Attack()
    {
        while (currentState == StateType.Attack)
        {
            while (isInvincible)
            {
                yield return null;
            }
            isAttacking = true;
            //Debug.Log("[공격 상태] 공격 시작");
            animator.SetTrigger("Attack");
            SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
            yield return new WaitForSeconds(1.0f);
            isAttacking = false;
            //Debug.Log("[공격 상태] 공격 종료, 상태 복귀");

            if (distanceToPlayer > chaseRange * 1.5f)
            {
                ChangeState(StateType.Idle);
                yield break;
            }
            else
            {
                ChangeState(StateType.Chase);
                yield break;
            }
        }
    }

    private IEnumerator Chase()
    {
        const float MAX_HEIGHT_DIFF = 0.8f;
        float chaseStartTime = Time.time;

        while (currentState == StateType.Chase)
        {
            while (isInvincible)
            {
                yield return null;
            }
            float heightDiff = Mathf.Abs(player.position.y - transform.position.y);

            if (heightDiff > MAX_HEIGHT_DIFF)
            {
                ChangeState(StateType.Idle);
                yield break;
            }

            if (distanceToPlayer < attackRange)
            {
                ChangeState(StateType.Attack);
                yield break;
            }

            if (distanceToPlayer > chaseRange * 1.5f)
            {
                ChangeState(StateType.Idle);
                yield break;
            }

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float directionSign = Mathf.Sign(directionToPlayer.x);
            spriteRenderer.flipX = directionSign < 0;

            animator.SetBool("IsWalk", true);
            transform.position += directionToPlayer * speed * Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator Patrol()
    {
        if (patrolPoints.Length > 0) animator.SetBool("IsWalk", true);
        else animator.SetBool("IsWalk", false);

        //float patrolTimer = 0;
        //float patrolDuration = 5.0f; // 순찰 상태 지속 시간
        while (currentState == StateType.Patrol/* && patrolTimer < patrolDuration*/)
        {
            while (isInvincible)
            {
                yield return null;
            }
            //patrolTimer += Time.deltaTime;

            if (distanceToPlayer < chaseRange)
            {
                ChangeState(StateType.Chase);
                yield break;
            }

            if (patrolPoints.Length > 0)
            {
                Transform targetPoint = patrolPoints[currentPoint];
                if (transform.position.x > targetPoint.position.x)
                {
                    direction = -1;
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (transform.position.x < targetPoint.position.x)
                {
                    direction = 1;
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);
                if (Vector3.Distance(transform.position, targetPoint.position) < 0.5f)
                {
                    currentPoint = (currentPoint + 1) % patrolPoints.Length;
                }
            }
            yield return null;
        }
    }

    private IEnumerator Idle()
    {
        const float MAX_HEIGHT_DIFF = 1.5f;
        const float MIN_IDLE_DURATION = 0.1f;
        animator.SetBool("IsWalk", false);
        yield return new WaitForSeconds(MIN_IDLE_DURATION);
        while (currentState == StateType.Idle)
        {
            while (isInvincible)
            {
                yield return null;
            }

            float heightDiff = Mathf.Abs(player.position.y - transform.position.y);
            if (heightDiff > MAX_HEIGHT_DIFF)
            {
                ChangeState(StateType.Patrol);
                yield break;
            }

            if (distanceToPlayer < chaseRange)
            {
                ChangeState(StateType.Chase);
                yield break;
            }
            else if (distanceToPlayer > chaseRange)
            {
                ChangeState(StateType.Patrol);
                yield break;
            }

            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Vector3 spawnPosition;
            if (collision.name == "AttackObjRight")
            {
                spawnPosition = collision.transform.position + transform.right * 0.5f;
                ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, spawnPosition, new Vector3(-4, 4, 4));
            }
            else
            {
                spawnPosition = collision.transform.position - transform.right * 0.5f;
                ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, spawnPosition, new Vector3(4, 4, 4));
            }

            Hit(5.0f, collision.name);
        }
    }

    #region comment

    //private IEnumerator ChangeColorTemporatily()
    //{
    //    SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
    //    objectRenderer.material.color = Color.red;
    //    yield return new WaitForSeconds(colorChangeDuration);
    //    objectRenderer.material.color = orginalColor;
    //}

    //IEnumerator AttackRoutine()
    //{
    //    isAttacking = true;
    //    //Debug.Log("[공격 상태] 공격 시작");
    //    animator.SetTrigger("Attack");
    //    SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
    //    yield return new WaitForSeconds(1.0f);
    //    currentState = StateType.Idle;
    //    isAttacking = false;
    //    //Debug.Log("[공격 상태] 공격 종료, 상태 복귀");
    //}

    //private void PatrolMovement()
    //{
    //    if (currentState == StateType.Patrol)
    //    {
    //        if (patrolPoints.Length > 0)
    //        {
    //            animator.SetBool("IsWalk", true);
    //            Transform targetPoint = patrolPoints[currentPoint];
    //            if (transform.position.x > targetPoint.position.x)
    //            {
    //                direction = -1;
    //                GetComponent<SpriteRenderer>().flipX = true;
    //            }
    //            else if (transform.position.x < targetPoint.position.x)
    //            {
    //                direction = 1;
    //                GetComponent<SpriteRenderer>().flipX = false;
    //            }
    //            transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);
    //            if (Vector3.Distance(transform.position, targetPoint.position) < 0.5f)
    //            {
    //                currentPoint = (currentPoint + 1) % patrolPoints.Length;
    //            }
    //        }
    //    }
    //}

    //IEnumerator Invincibility()
    //{
    //    float elapsedTime = 0f;
    //    float blinkInterval = 0.2f;

    //    Color originalColor = spriteRenderer.color;

    //    while (elapsedTime < invincibilityDuration)
    //    {
    //        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);
    //        yield return new WaitForSeconds(blinkInterval);
    //        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
    //        yield return new WaitForSeconds(blinkInterval);
    //        elapsedTime += blinkInterval * 2;
    //    }

    //    spriteRenderer.color = originalColor;
    //    cld.enabled = false;
    //    cld.enabled = true;
    //    isInvincible = false;
    //}
    #endregion

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
}
