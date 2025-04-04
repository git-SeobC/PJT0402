using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int coinCount = 0;

    public Text coinText;

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

    public void AddCoin(int amount)
    {
        coinCount += amount;
        coinText.text = coinCount.ToString();
        SoundManager.Instance.PlaySFX(SFXType.PickupItemSFX);
        PlayerPrefs.SetInt("Coin", coinCount);
    }

    public int GetCoinCount()
    {
        return coinCount;
    }


    public void ResetCoin()
    {
        coinCount = 0;
        PlayerPrefs.SetInt("Coin", coinCount);
    }
}
