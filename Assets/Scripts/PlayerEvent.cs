using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public GameObject ArrowKeyObj;
    public GameObject JumpKeyObj;
    public GameObject AttackKeyObj;

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
    }
}
