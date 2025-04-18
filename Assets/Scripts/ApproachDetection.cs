using UnityEngine;

/// <summary>
/// 트랩 접근 확인용 스크립트
/// </summary>
public class ApproachDetection : MonoBehaviour
{
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("IsActive", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("IsActive", false);
        }
    }
}