using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    public static SceneManagerController Instance { get; private set; }

    public Image basePanel;
    public Image gameOverPanel;
    public Text gameOverText;
    public Button retryBtn;
    public Button quitBtn;

    public float fadeDuration = 1.0f;
    public string nextSceneName;
    private bool isFading = false;
    public int savePointMapindex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(BGMType.MenuBGM, 1.0f);
    }

    public void StartSeneTransition(string sceneName)
    {
        if (!isFading)
        {
            nextSceneName = sceneName;
            Debug.Log(nextSceneName);
            StartCoroutine(FadeInAndLoadScene());
        }
    }

    public void GameOver()
    {
        SoundManager.Instance.StopBGM(0);
        basePanel.gameObject.SetActive(true);
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // basePanel 페이드인
        yield return StartCoroutine(FadeImage(0, 1, fadeDuration));

        // GameOver 패널 활성화
        gameOverPanel.gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX(SFXType.GameOverSoundSFX);
        // "GAME OVER" 타이핑 효과
        yield return StartCoroutine(TypingGameOver(gameOverText, "GAME OVER"));

        // 버튼 활성화
        retryBtn.gameObject.SetActive(true);
        quitBtn.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RetryBtnClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        Time.timeScale = 1.0f;
        retryBtn.gameObject.SetActive(false);
        quitBtn.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        basePanel.gameObject.SetActive(false);
        SceneManager.LoadScene("LoadingScene");
        GameManager.Instance.ResetGame();
    }

    public void QuitBtnClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        retryBtn.gameObject.SetActive(false);
        quitBtn.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        basePanel.gameObject.SetActive(false);
        Application.Quit();
    }

    IEnumerator TypingGameOver(Text text, string content)
    {
        text.text = ""; // 현재 화면 메세지 비움

        int typingCount = 0; // 타이핑 카운트 0 초기화
        float elapsedTime = 0f;
        float typingDelay = 0.2f;

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
        Color finalTextColor = gameOverText.color;
        finalTextColor.a = 1;
        gameOverText.color = finalTextColor;

        // 현재 카운트가 컨텐츠의 길이와 다르다면 
        while (typingCount != content.Length)
        {
            if (typingCount < content.Length)
            {
                text.text += content[typingCount].ToString();
                // 현재 카운트에 해당하는 단어 하나를 메세지 텍스트 UI에 전달
                typingCount++;
                // 카운트를 1 증가
            }
            yield return new WaitForSeconds(typingDelay);
            // 현재의 딜레이만큼 대기
        }
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        //Debug.Log("FadeOut Start ----------------------------");
        float elapsedTime = 0f;
        Color panelColor = basePanel.color;

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
        //Debug.Log("FadeOut End ----------------------------");
    }

    IEnumerator FadeInAndLoadScene()
    {
        isFading = true;
        basePanel.gameObject.SetActive(true);

        yield return StartCoroutine(FadeImage(0, 1, fadeDuration));

        //SceneManager.LoadScene("LoadingScene");

        yield return StartCoroutine(LoadLoadingAndNextScene());

        yield return StartCoroutine(FadeImage(1, 0, fadeDuration));

        isFading = false;
        basePanel.gameObject.SetActive(false);
    }

    IEnumerator LoadLoadingAndNextScene()
    {
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        loadingSceneOp.allowSceneActivation = true;

        while (!loadingSceneOp.isDone)
        {
            yield return null;
        }

        AsyncOperation nextSceneOp = SceneManager.LoadSceneAsync(nextSceneName);
        nextSceneOp.allowSceneActivation = false;

        float minDisplayTime = 2.0f;
        float loadingStartTime = Time.time;
        bool minTimePassed = false;

        Slider loadingSlider = GameObject.Find("LoadingSlider")?.GetComponent<Slider>();

        //while (!loadingSceneOp.isDone)
        //{
        //    if (loadingSceneOp.progress >= 0.9f)
        //    {
        //        loadingSceneOp.allowSceneActivation = true;
        //    }
        //    yield return null;
        //}

        while (!nextSceneOp.isDone)
        {
            if (Time.time - loadingStartTime >= minDisplayTime)
            {
                minTimePassed = true;
            }

            //float progress = Mathf.Clamp01(nextSceneOp.progress / 0.9f);

            //if (loadingSlider != null)
            //{
            //    loadingSlider.value = progress;
            //}

            if (minTimePassed && nextSceneOp.progress >= 0.9f && loadingSlider.value >= 0.9f)
            {
                nextSceneOp.allowSceneActivation = true; // 씬 활성화
            }

            yield return null;
        }


        if (SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            SceneManager.UnloadSceneAsync("LoadingScene");
        }
        else
        {
            Debug.Log("로딩씬 not Load");
        }
    }

    public void MenuSelectSFX()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuSelectSFX);
    }

    public void ExitScene()
    {
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        Application.Quit();
    }

    public void StartGameMenu()
    {
        SoundManager.Instance.StopBGM(0);
        SoundManager.Instance.PlaySFX(SFXType.MenuClickSFX);
        //Debug.Log("게임시작");
        StartSeneTransition("Scene1");
    }
}