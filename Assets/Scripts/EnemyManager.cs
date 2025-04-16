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
    public float attackRange = 1.5f;
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

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        orginalColor = objectRenderer.material.color;
        startPos = transform.position;
        //int randomChoice = Random.Range(0, 1);
        animator = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        //stateChangeRoutine = StartCoroutine(RandomStateChanger());
    }

    private void Update()
    {
        if (enemyType == EnemyType.None || player == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            if (currentState != StateType.Attack)
            {
                StopAllCoroutines();
                currentState = StateType.Attack;
                StartCoroutine(AttackRoutine());
            }
            return;
        }

        if (distanceToPlayer <= chaseRange)
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
            transform.position += directionToPlayer * Time.deltaTime;
            return;
        }

        //if ((stateType == StateType.ChaseWalk || stateType == StateType.ChaseRun) && distanceToPlayer > chaseRange)
        //{
        //    //Debug.Log("[상태 전환] 추적 종료");
        //    stateType = StateType.Idle;
        //    if (stateChangeRoutine == null)
        //    {
        //        stateChangeRoutine = StartCoroutine(RandomStateChanger());
        //    }
        //}

        if (currentState == StateType.Attack) return;
        PatrolMovement();
    }

    #region MyRegion
    //public void ChangeState(StateType newState)
    //{
    //    if (stateChangeRoutine != null)
    //    {
    //        StopCoroutine(stateChangeRoutine);
    //    }

    //    currentState = newState;

    //    switch (currentState)
    //    {
    //        case StateType.Idle:
    //            stateChangeRoutine = StartCoroutine(Idle());
    //            break;
    //        case StateType.PatrolWalk:
    //            stateChangeRoutine = StartCoroutine(Patrol());
    //            break;
    //        case StateType.ChaseWalk:
    //            stateChangeRoutine = StartCoroutine(Chase());
    //            break;
    //        case StateType.Attack:
    //            stateChangeRoutine = StartCoroutine(Attack());
    //            break;
    //        case StateType.Hit:
    //            stateChangeRoutine = StartCoroutine(Hit());
    //            break;
    //        case StateType.Death:
    //            stateChangeRoutine = StartCoroutine(Death());
    //            break;
    //    }
    //}

    //private IEnumerator Death()
    //{
    //    throw new NotImplementedException();
    //}

    //private IEnumerator Hit()
    //{
    //    throw new NotImplementedException();
    //}

    //private IEnumerator Attack()
    //{
    //    throw new NotImplementedException();
    //}

    //private IEnumerator Chase()
    //{
    //    throw new NotImplementedException();
    //}

    //private IEnumerator Patrol()
    //{
    //    throw new NotImplementedException();
    //}

    //private IEnumerator Idle()
    //{
    //    animator.Play("EnemyIdle");

    //    while (currentState == StateType.Idle)
    //    {
    //        animator.SetBool("IsWalk", false);
    //        distanceToPlayer = Vector2.Distance(transform.position, player.position);

    //        if (distanceToPlayer < chaseRange)
    //        {
    //            ChangeState(StateType.ChaseWalk);
    //            yield break;
    //        }
    //        else
    //        {
    //            ChangeState(StateType.PatrolWalk);
    //            yield break;
    //        }
    //    }
    //} 
    #endregion

    private void PatrolMovement()
    {
        if (currentState == StateType.PatrolWalk)
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

            animator.SetTrigger("Hit");

            enemyHp -= 5.0f;
            if (enemyHp > 0) StartCoroutine(ChangeColorTemporatily());
            else Destroy(gameObject);
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
