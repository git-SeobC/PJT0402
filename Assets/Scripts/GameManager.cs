using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public GameObject[] enemyGroup;

    public int currentMapIndex = 1;
    public bool changeMap = false;

    [Header("Ending Settings")]
    [SerializeField] private Canvas endingCanvas;
    [SerializeField] private Text endingText;
    [SerializeField] private Text creditTextPrefab;
    [SerializeField] private Transform creditContainer;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float perspectiveScale = 0.9f;
    [SerializeField] private Transform vanishingPointPanel;

    public Image endingPanel;
    public Button retryBtn;
    public Button quitBtn;

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
        currentMapIndex = 1;
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

    private void EnemyObjActiveSet(int pIndex, bool pSet)
    {
        foreach (Transform child in enemyGroup[pIndex].transform)
        {
            child.gameObject.SetActive(pSet);
        }
    }

    public IEnumerator SetPlayerStartPosition()
    {
        SceneManagerController.Instance.savePointMapindex = currentMapIndex;

        changeMap = true;
        SoundManager.Instance.StopBGM(0f);

        // 기존 맵 적 오브젝트 비활성화
        EnemyObjActiveSet(currentMapIndex, false);

        yield return StartCoroutine(LoadingStart());

        currentMapIndex++;
        SceneManagerController.Instance.savePointMapindex = currentMapIndex;

        if (enemyGroup[currentMapIndex] != null)
        {
            EnemyObjActiveSet(currentMapIndex, true);
        }

        cinemachineConfiner.enabled = false;
        cinemachineConfiner.BoundingShape2D = mapBoundaries[currentMapIndex];
        yield return StartCoroutine(DelayedTeleport(currentMapIndex));
        yield return new WaitForSeconds(1.0f);
        cinemachineConfiner.enabled = true;
        yield return new WaitForSeconds(0.5f);

        loadingDoor.gameObject.SetActive(false);
        loadingSlider.gameObject.SetActive(false);

        if (currentMapIndex == 2)
        {
            SoundManager.Instance.PlayBGM(BGMType.Scene2BGM, 0f);
        }

        basePanel.gameObject.SetActive(false);
        LoadingUI.SetActive(false);
        UI.SetActive(true);
        changeMap = false;
        yield return StartCoroutine(FadeImage(1, 0, 1.0f, basePanel));
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

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.ManualUpdate();
    }

    IEnumerator LoadingStart()
    {
        UI.SetActive(false);
        LoadingUI.SetActive(true);
        basePanel.gameObject.SetActive(true);

        yield return StartCoroutine(FadeImage(0, 1, 1.0f, basePanel));

        loadingDoor.gameObject.SetActive(true);
        loadingSlider.gameObject.SetActive(true);

        yield return StartCoroutine(LoadingCoroutine());
    }

    private IEnumerator LoadingCoroutine()
    {
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
            yield return null;
        }
        SoundManager.Instance.PlaySFX(SFXType.LoadingFinishSFX);
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha, float duration, Image image)
    {
        float elapsedTime = 0f;
        Color panelColor = image.color;
        panelColor.a = startAlpha;
        image.color = panelColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            panelColor.a = newAlpha;
            image.color = panelColor;
            yield return null;
        }

        panelColor.a = endAlpha;
        image.color = panelColor;
    }

    public void ResetGame()
    {
        //if (currentMapIndex == 2)
        //{
        //    currentMapIndex = 1;
        //    StartCoroutine(SetPlayerStartPosition());
        //}
        //else currentMapIndex = 1;
        playerController.playerHP = 3;
        SetLifeUI();
    }

    string endingStory = "핀은 제이크를 찾기 위해 얼음대왕 성 침투에 성공하였지만...";
    string toBeContinued = "다음 편에 계에속";
    string initialMessage = "Thank you for playing";
    string[] credits = {
            "제작사",
            "SBCamp",
            "기획",
            "서병찬",
            "프로그래밍",
            "서병찬",
            "아트",
            "여러 에셋 줍줍",
            "사운드",
            "손강사님",
            "여러 에셋 줍줍",
            "플레이어",
            "여러분들께 감사드립니다."
    };

    public void StartEndingSequence()
    {
        changeMap = true; // 플레이어 움직임 방지
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // 플레이어 정지
        EnemyObjActiveSet(currentMapIndex, false); // 현재 맵 적 오브젝트들 Active False
        SoundManager.Instance.StopBGM(0);
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        endingCanvas.gameObject.SetActive(true);
        UI.SetActive(false);

        SoundManager.Instance.PlayBGM(BGMType.EndingCreditsBGM, 0.5f);
        yield return StartCoroutine(FadeImage(0, 1, fadeDuration, endingPanel)); // 엔딩 패널 페이드 인

        // 1. 초기 메시지 표시
        yield return StartCoroutine(Typing(endingText, endingStory, 0.1f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(Typing(endingText, ""));
        yield return StartCoroutine(Typing(endingText, toBeContinued, 0.05f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(Typing(endingText, ""));

        // 2. 크레딧 롤 시작
        StartCoroutine(PlayCredits());
        yield return new WaitForSeconds(14f);

        SoundManager.Instance.StopBGM(0);
        SoundManager.Instance.SetSFXVoluime(0.3f);
        SoundManager.Instance.PlaySFX(SFXType.GameClearSoundSFX);

        yield return StartCoroutine(Typing(endingText, initialMessage));
        yield return new WaitForSeconds(3.3f);

        retryBtn.gameObject.SetActive(true);
        quitBtn.gameObject.SetActive(true);
        SoundManager.Instance.SetSFXVoluime(1f);
    }

    private IEnumerator PlayCredits()
    {
        float spacing = Screen.height * 0.2f;
        float startY = -Screen.height * 0.5f;

        foreach (string credit in credits)
        {
            // 크레딧 텍스트 생성
            Text creditText = Instantiate(creditTextPrefab, creditContainer);
            creditText.text = credit;

            // 초기 위치 설정 (화면 아래 밖)
            creditText.rectTransform.anchoredPosition = new Vector2(0, startY);
            startY -= spacing;

            // 크레딧 애니메이션 시작
            StartCoroutine(AnimateCredit(creditText));
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator AnimateCredit(Text creditText)
    {
        float duration = 8f;
        float elapsed = 0f;
        RectTransform rect = creditText.rectTransform;
        Color originalColor = creditText.color;
        Vector3 vanishingPoint = vanishingPointPanel.transform.position;

        while (elapsed < duration)
        {
            // 위치 이동 (위로 스크롤)
            rect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // 원근 효과 계산
            float depth = Mathf.Abs(vanishingPoint.z - rect.position.z);
            float scale = Mathf.Clamp(1 - (elapsed / duration) * perspectiveScale, 0.1f, 1f);
            rect.localScale = Vector3.one * scale;

            // 투명도 조절
            float alpha = 1 - (elapsed / duration);
            creditText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // 소실점 방향으로 이동
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(rect.position);
            rect.position = Vector3.Lerp(rect.position, vanishingPoint, elapsed / duration * 0.01f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(creditText.gameObject);
    }


    IEnumerator Typing(Text text, string content, float typingDelay = 0.1f)
    {
        text.text = ""; // 현재 화면 메세지 비움

        int typingCount = 0; // 타이핑 카운트 0 초기화
        float elapsedTime = 0f;

        // 텍스트 페이드 인
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            Color textColor = text.color;
            textColor.a = alpha;
            text.color = textColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 텍스트를 완전히 불투명하게 설정
        Color finalTextColor = endingText.color;
        finalTextColor.a = 1;
        endingText.color = finalTextColor;

        // 현재 카운트가 컨텐츠의 길이와 다르다면 
        while (typingCount != content.Length)
        {
            if (typingCount < content.Length)
            {
                text.text += content[typingCount].ToString();
                // 현재 카운트에 해당하는 단어 하나를 메세지 텍스트 UI에 전달
                SoundManager.Instance.PlaySFX(SFXType.TypingSFX);
                typingCount++;
                // 카운트를 1 증가
            }
            yield return new WaitForSeconds(typingDelay);
            // 현재의 딜레이만큼 대기
        }
    }

    public void RetryBtnClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        Destroy(SceneManagerController.Instance);
        SceneManager.LoadScene("Menu");
    }

    public void QuitBtnClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        Application.Quit();
    }

    public void MenuSelectSFX()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuSelectSFX);
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
