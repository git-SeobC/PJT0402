using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    private PlayerController playerController;
    public GameObject player;
    public GameObject[] Life = new GameObject[3];

    private const string COIN_KEY = "CoinCount";
    private const string DAMAGE_KEY = "PlayerDamage";
    private const string ATTACK_SPEED_KEY = "PlayerAttackSpeed";
    private const string MOVE_SPEED_KEY = "PlayerMoveSpeed";
    private const string HP_KEY = "PlayerHP";

    public int coinCount = 0;


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

    //public void AddCoin(int amount)
    //{
    //    coinCount += amount;
    //    coinText.text = coinCount.ToString();
    //    SoundManager.Instance.PlaySFX(SFXType.ItemGet);
    //}

    //public bool UseCoin(int amount)
    //{

    //}

    public int GetCoinCount()
    {
        return coinCount;
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(COIN_KEY, coinCount);
        PlayerPrefs.Save();
    }

    private void LoadCoin()
    {
        coinCount = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

}
