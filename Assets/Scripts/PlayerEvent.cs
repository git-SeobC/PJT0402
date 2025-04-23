using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public GameObject ArrowKeyObj;
    public GameObject JumpKeyObj;
    public GameObject AttackKeyObj;
    public GameObject UpKeyObj;

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
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //SceneManagerController.Instance.LoadNextScene();
                GameManager.Instance.SetPlayerStartPosition(2);
            }
        }
    }
}
