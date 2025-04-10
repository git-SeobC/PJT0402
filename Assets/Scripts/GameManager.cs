using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    private PlayerController playerController;
    public GameObject player;
    public GameObject[] Life = new GameObject[3];



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();

        SetLifeUI();
    }

    public void SetLifeUI()
    {
        for (int i = 0; i < Life.Length; i++)
        {
            Life[i].SetActive(i < playerController.playerHP);
        }
    }
}
