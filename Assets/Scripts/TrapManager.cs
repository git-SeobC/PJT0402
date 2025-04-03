using UnityEngine;

public enum TrapType
{
    HorizontalMoveGround,
    VerticalMoveGround,
}

public class TrapManager : MonoBehaviour
{
    public float speed = 2.0f;
    public float maxDistance = 3.0f;
    private Vector3 startPos;
    private int direction = 1;
    public TrapType currentType;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (currentType == TrapType.HorizontalMoveGround)
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
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
        
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
        
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trap Trigger Enter");
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(gameObject.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trap Trigger Exit");
        if (collision.CompareTag("Player") && collision.transform.parent.gameObject.activeInHierarchy)
        {
            collision.transform.SetParent(null);
        }
    }
}