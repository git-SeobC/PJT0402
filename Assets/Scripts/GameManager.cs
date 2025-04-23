using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.AppUI.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private PlayerController playerController;
    public GameObject player;
    public GameObject[] Life = new GameObject[3];
    public PolygonCollider2D[] mapBoundaries; // 카메라 제한 아웃라인 콜라이더
    public CinemachineConfiner2D cinemachineConfiner; // 시네머신 컴포넌트
    public Transform[] mapStartPos; // 맵 시작위치 배열

    public GameObject UI;
    public GameObject LoadingUI;
    public Slider loadingSlider;
    private Sprite[] images;
    public GameObject handle;
    public Image basePanel;
    public Image loadingDoor;

    //private const string COIN_KEY = "CoinCount";
    //private const string DAMAGE_KEY = "PlayerDamage";
    //private const string ATTACK_SPEED_KEY = "PlayerAttackSpeed";
    //private const string MOVE_SPEED_KEY = "PlayerMoveSpeed";
    //private const string HP_KEY = "PlayerHP";
    //public int coinCount = 0;

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
        images = Resources.LoadAll<Sprite>("loadingFinn");
        SetLifeUI();
    }

    public void SetLifeUI()
    {
        for (int i = 0; i < Life.Length; i++)
        {
            Life[i].SetActive(i < playerController.playerHP);
        }
    }

    public void SetPlayerStartPosition(int pIndex)
    {
        StartCoroutine(LoadingStart());

        cinemachineConfiner.BoundingShape2D = mapBoundaries[pIndex];

        StartCoroutine(DelayedTeleport(pIndex));
    }

    private IEnumerator DelayedTeleport(int index)
    {
        yield return new WaitForSeconds(0.5f);

        player.transform.position = mapStartPos[index].position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    IEnumerator LoadingStart()
    {
        UI.SetActive(false);

        LoadingUI.SetActive(true);
        basePanel.enabled = true;

        StartCoroutine(FadeImage(0, 1, 1.0f));
        yield return new WaitForSeconds(1.0f);

        StartCoroutine(LoadingCoroutine());
        yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator LoadingCoroutine()
    {
        yield return null;

        float timer = 0f;
        float animationTimer = 0f;
        float loadingTime = 1.0f;
        float animationFrameRate = 10f;
        float progress;
        int currentFrame = 0;
        Image handlerImage = handle.GetComponent<Image>();

        while (timer < 2.0f)
        {
            progress = Mathf.Clamp01(timer / loadingTime);
            loadingSlider.value = progress;

            animationTimer += Time.deltaTime;
            if (animationTimer >= 1f / animationFrameRate)
            {
                animationTimer -= 1f / animationFrameRate;
                currentFrame++;
                if (currentFrame >= images.Length)
                    currentFrame = 0; // 반복
                handlerImage.sprite = images[currentFrame];
                SoundManager.Instance.PlaySFX(SFXType.PlayerStepSFX);
            }
            timer += Time.deltaTime;
        }

        yield return null;
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color panelColor = basePanel.color;
        panelColor.a = startAlpha;
        basePanel.color = panelColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            panelColor.a = newAlpha;
            basePanel.color = panelColor;
            yield return null;
        }

        panelColor.a = endAlpha;
        basePanel.color = panelColor;
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

    //public int GetCoinCount()
    //{
    //    return coinCount;
    //}

    //private void SaveCoin()
    //{
    //    PlayerPrefs.SetInt(COIN_KEY, coinCount);
    //    PlayerPrefs.Save();
    //}

    //private void LoadCoin()
    //{
    //    coinCount = PlayerPrefs.GetInt(COIN_KEY, 0);
    //}

}
