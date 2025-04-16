using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngineInternal;

public enum EnemyType
{
    None, Orc
}

public enum StateType
{
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
    public float chaseRange = 5.0f;
    public float attackRange = 1.0f;
    public bool isAttacking = false;

    private float stateChangeInterval = 3.0f;
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
    private bool isInvincible = false;
    public float invincibilityDuration = 1.0f;
    public float knockbackForce = 5.0f;
    private bool isKnockback = false;
    public float knockbackDuration = 0.2f;
    private CapsuleCollider2D cld;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        orginalColor = objectRenderer.material.color;
        startPos = transform.position;
        //int randomChoice = Random.Range(0, 1);
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cld = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        ChangeState(currentState);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        //stateChangeRoutine = StartCoroutine(RandomStateChanger());
    }

    private void Update()
    {
        if (enemyType == EnemyType.None || player == null) return;

        if (enemyType == EnemyType.Orc)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            distanceToPlayer = Vector2.Distance(transform.position, player.position);

            return;

            if (enemyHp <= 0) currentState = StateType.Death;

            if (currentState == StateType.Hit)
            {
                if (isInvincible == true) return;
                StopAllCoroutines();
                SoundManager.Instance.PlaySFX(SFXType.PlayerHitSFX);
                ParticleManager.Instance.ParticlePlay(ParticleType.PlayerDamage, transform.position, new Vector3(4, 4, 4));
                StartCoroutine(Invincibility());
                Vector2 knockbackDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
                animator.SetTrigger("Hit");
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                StartCoroutine(KnockbackCoroutine());
                return;
            }
            else if (currentState == StateType.Death)
            {

            }

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && isAttacking == false)
            {
                if (currentState != StateType.Attack)
                {
                    StopAllCoroutines();
                    currentState = StateType.Attack;
                    StartCoroutine(AttackRoutine());
                }
                return;
            }

            if (distanceToPlayer <= chaseRange && isAttacking == false)
            {
                if (currentState != StateType.Chase)
                {
                    if (stateChangeRoutine != null)
                    {
                        StopCoroutine(stateChangeRoutine);
                    }
                    //Debug.Log($"[상태 전환] 추적 상태 : {stateType}");
                }
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                if (directionToPlayer.x > 0)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }

                animator.SetBool("IsWalk", true);
                transform.position += directionToPlayer * speed * Time.deltaTime;
                return;
            }

            //if (currentState == StateType.Chase && distanceToPlayer > chaseRange * 2)
            //{
            //    //Debug.Log("[상태 전환] 추적 종료");
            //    currentState = StateType.Patrol;
            //}

            if (currentState == StateType.Attack) return;
            PatrolMovement();
        }
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        float elapsedTime = 0f;
        float blinkInterval = 0.2f;

        Color originalColor = spriteRenderer.color;

        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval * 2;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
        cld.enabled = false;
        cld.enabled = true;
    }

    IEnumerator KnockbackCoroutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
        currentState = StateType.Idle;
    }

    #region MyRegion
    public void ChangeState(StateType newState)
    {
        if (stateChangeRoutine != null)
        {
            StopCoroutine(stateChangeRoutine);
        }

        currentState = newState;
        animator.SetBool("IsWalk", false);

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
            case StateType.Hit:
                stateChangeRoutine = StartCoroutine(Hit());
                break;
            case StateType.Death:
                stateChangeRoutine = StartCoroutine(Death());
                break;
        }
    }

    private IEnumerator Death()
    {
        throw new NotImplementedException();
    }

    private IEnumerator Hit()
    {
        throw new NotImplementedException();
    }

    private IEnumerator Attack()
    {
        while (currentState == StateType.Attack)
        {
            isAttacking = true;
            //Debug.Log("[공격 상태] 공격 시작");
            animator.SetTrigger("Attack");
            SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
            yield return new WaitForSeconds(1.0f);
            isAttacking = false;
            //Debug.Log("[공격 상태] 공격 종료, 상태 복귀");


        }
    }

    private IEnumerator Chase()
    {
        while (currentState == StateType.Chase)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Math.Abs(player.position.y - transform.position.y) < 0.8f)
            {
                if (directionToPlayer.x > 0)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }

                animator.SetBool("IsWalk", true);
                transform.position += directionToPlayer * speed * Time.deltaTime;
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance < attackRange)
                {
                    ChangeState(StateType.Attack);
                    yield break;
                }
                else if (distance > chaseRange)
                {
                    ChangeState(StateType.Idle);
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator Patrol()
    {
        while (currentState == StateType.Patrol)
        {
            if (patrolPoints.Length > 0)
            {
                animator.SetBool("IsWalk", true);
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

                float distance = Vector2.Distance(transform.position, player.position);

                if (distance < chaseRange)
                {
                    ChangeState(StateType.Chase);
                    yield break;
                }
                else if (distance < attackRange)
                {
                    ChangeState(StateType.Attack);
                    yield break;
                }
            }
            else
            {
                ChangeState(StateType.Idle);
            }
            yield return null;
        }
    }

    private IEnumerator Idle()
    {
        animator.Play("EnemyIdle");

        while (currentState == StateType.Idle)
        {
            animator.SetBool("IsWalk", false);
            distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < chaseRange)
            {
                ChangeState(StateType.Chase);
                yield break;
            }
            else
            {
                ChangeState(StateType.Patrol);
                yield break;
            }
        }
    }
    #endregion

    private void PatrolMovement()
    {
        if (currentState == StateType.Patrol)
        {
            if (patrolPoints.Length > 0)
            {
                animator.SetBool("IsWalk", true);
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

            enemyHp -= 5.0f;
            if (enemyHp > 0) /*StartCoroutine(ChangeColorTemporatily());*/
                currentState = StateType.Hit;
            //else Destroy(gameObject);
        }
    }

    private IEnumerator ChangeColorTemporatily()
    {
        SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
        objectRenderer.material.color = Color.red;
        yield return new WaitForSeconds(colorChangeDuration);
        objectRenderer.material.color = orginalColor;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        //Debug.Log("[공격 상태] 공격 시작");
        animator.SetTrigger("Attack");
        SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
        yield return new WaitForSeconds(1.0f);
        currentState = StateType.Idle;
        isAttacking = false;
        //Debug.Log("[공격 상태] 공격 종료, 상태 복귀");
    }
}
