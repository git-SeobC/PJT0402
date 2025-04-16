using System.Collections;
using UnityEngine;

public enum TrapType
{
    HorizontalMoveGround,
    VerticalMoveGround,
    SpikeTrap,
    HorizontalMoveTrap,
    BrickTrap,
}

public class TrapManager : MonoBehaviour
{
    public float speed = 2.0f;
    public float maxDistance = 3.0f;
    private Vector3 startPos;
    private int direction = 1;
    public TrapType currentType;

    private bool isBreaking = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (currentType == TrapType.BrickTrap) return;

        if (currentType == TrapType.HorizontalMoveGround || currentType == TrapType.HorizontalMoveTrap)
        {
            if (transform.position.x > startPos.x + maxDistance)
            {
                direction = -1;
            }
            else if (transform.position.x < startPos.x - maxDistance)
            {
                direction = 1;
            }
            transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);
        }
        else if (currentType == TrapType.VerticalMoveGround)
        {
            if (transform.position.y > startPos.y + maxDistance)
            {
                direction = -1;
            }
            else if (transform.position.y < startPos.y - maxDistance)
            {
                direction = 1;
            }
            transform.position += new Vector3(0, speed * direction * Time.deltaTime, 0);
        }
        else if (currentType == TrapType.SpikeTrap)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (currentType == TrapType.HorizontalMoveGround || currentType == TrapType.VerticalMoveGround))
        {
            collision.transform.SetParent(gameObject.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (currentType == TrapType.HorizontalMoveGround || currentType == TrapType.VerticalMoveGround) && collision.transform.parent.gameObject.activeInHierarchy)
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && (currentType == TrapType.BrickTrap) && isBreaking == false)
        {
            StartCoroutine(BrickBreakingEvent());
        }
    }

    IEnumerator BrickBreakingEvent()
    {
        isBreaking = true;
        Vector3 originalPos = transform.position;
        float shakeDuration = 0.5f;
        float elapsed = 0f;
        float shakeIntensity = 0.05f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 shakeOffset = Random.insideUnitCircle * shakeIntensity;
            transform.position = originalPos + shakeOffset;
            yield return null;
        }

        GetComponent<Animator>().SetTrigger("Broken");

        gameObject.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        gameObject.SetActive(false);
        isBreaking = false;
    }

    /// <summary>
    /// 애니메이션 이벤트용 메소드
    /// </summary>
    public void SpikeTrapSFXOn()
    {
        SoundManager.Instance.PlaySFX(SFXType.SpikeTrapSFX);
    }
}