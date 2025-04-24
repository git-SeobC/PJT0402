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
                isInputLocked = true;
                isInPortal = false;
                StartCoroutine(GameManager.Instance.SetPlayerStartPosition());
                StartCoroutine(UnlockInputAfterDelay(3.0f));
            }
            else if (isEnding && Input.GetKeyDown(KeyCode.UpArrow))
            {
                isInputLocked = true;
                isEnding = false;
                GameManager.Instance.StartEndingSequence();
                StartCoroutine(UnlockInputAfterDelay(3.0f));
            }
            //else if (Input.GetKeyDown(KeyCode.E))
            //{
            //    GameManager.Instance.StartEndingSequence();
            //}
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
        // activeInHierarchy 부모 오브젝트가 Off되어있으면 false임 -> 실질적으로 On이 가능한 상태인지 확인하는 용도
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

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    Debug.Log($"{collision.CompareTag("Portal")}");
    //    if (collision.CompareTag("Portal"))
    //    {
    //        if (Input.GetKeyDown(KeyCode.UpArrow))
    //        {
    //            //SceneManagerController.Instance.LoadNextScene();
    //            GameManager.Instance.SetPlayerStartPosition(2);
    //        }
    //    }
    //}
}
