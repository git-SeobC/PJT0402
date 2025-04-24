using System.Collections;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public GameObject ArrowKeyObj;
    public GameObject JumpKeyObj;
    public GameObject AttackKeyObj;
    public GameObject UpKeyObj;

    public bool isInPortal = false;
    private bool isEnding = false;

    private bool isInputLocked = false;

    private void Update()
    {
        if (isInputLocked == false)
        {
            if (isInPortal && Input.GetKeyDown(KeyCode.UpArrow))
            {
                SoundManager.Instance.PlaySFX(SFXType.IntoPortalSFX);
                isInputLocked = true;
                isInPortal = false;
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
                PlayerController.Instance.animator.SetBool("IsWalking", false);
                PlayerController.Instance.animator.Play("Pin_Idle");
                StartCoroutine(GameManager.Instance.SetPlayerStartPosition());
                StartCoroutine(UnlockInputAfterDelay(3.0f));
            }
            else if (isEnding && Input.GetKeyDown(KeyCode.UpArrow))
            {
                SoundManager.Instance.PlaySFX(SFXType.IntoPortalSFX);
                isInputLocked = true;
                isEnding = false;
                GameManager.Instance.StartEndingSequence();
                StartCoroutine(UnlockInputAfterDelay(3.0f));
            }
            else if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.E))
            {
                SoundManager.Instance.PlaySFX(SFXType.IntoPortalSFX);
                GameManager.Instance.StartEndingSequence();
            }
        }
    }

    private IEnumerator UnlockInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInputLocked = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "TutorialEvent1")
        {
            ArrowKeyObj.SetActive(true);
            if (SceneManagerController.Instance.savePointMapindex == 2)
            {
                StartCoroutine(GameManager.Instance.SetPlayerStartPosition());
            }
        }
        else if (collision.name == "TutorialEvent2")
        {
            JumpKeyObj.SetActive(true);
        }
        else if (collision.name == "TutorialEvent3")
        {
            AttackKeyObj.SetActive(true);
        }
        else if (collision.name == "Portal")
        {
            UpKeyObj.SetActive(true);
            isInPortal = true;
        }
        else if (collision.name == "Ending")
        {
            isEnding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "TutorialEvent1" && collision.gameObject.activeInHierarchy)
        {
            ArrowKeyObj.SetActive(false);
        }
        else if (collision.name == "TutorialEvent2" && collision.gameObject.activeInHierarchy)
        {
            JumpKeyObj.SetActive(false);
        }
        else if (collision.name == "TutorialEvent3" && collision.gameObject.activeInHierarchy)
        {
            AttackKeyObj.SetActive(false);
        }
        else if (collision.name == "Portal" && collision.gameObject.activeInHierarchy)
        {
            UpKeyObj.SetActive(false);
            isInPortal = false;
        }
        else if (collision.name == "Ending")
        {
            isEnding = false;
        }
    }
}
