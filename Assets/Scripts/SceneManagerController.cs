using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class SceneManagerController : MonoBehaviour
{
    public static SceneManagerController Instance { get; private set; }

    public Image panel;

    public float fadeDuration = 1.0f;
    public string nextSceneName;
    private bool isFading = false;

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

    public void StartSeneTransition(string sceneName)
    {
        if (!isFading)
        {
            nextSceneName = sceneName;
            StartCoroutine(FadeInAndLoadScene());
        }
    }

    IEnumerator FadeInAndLoadScene()
    {
        isFading = true;
        panel.gameObject.SetActive(true);

        //FadeImage(0, 1, fadeDuration);

        SceneManager.LoadScene("LoadingScene");

        //yield return StartCoroutine(LoadLoadingAndNextScene());

        yield return StartCoroutine(FadeImage(1, 0, fadeDuration));

        isFading = false;
        panel.gameObject.SetActive(false);
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        Debug.Log("FadeOut Start ----------------------------");
        float elapsedTime = 0f;
        Color panelColor = panel.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            panelColor.a = newAlpha;
            panel.color = panelColor;
            yield return null;
        }

        panelColor.a = endAlpha;
        panel.color = panelColor;
        Debug.Log("FadeOut End ----------------------------");
    }

    IEnumerator LoadLoadingAndNextScene()
    {
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        loadingSceneOp.allowSceneActivation = false;

        while (!loadingSceneOp.isDone)
        {
            if (loadingSceneOp.progress >= 0.9f)
            {
                loadingSceneOp.allowSceneActivation = true;
            }
            yield return null;
        }

        Slider loadingSlider = null;
        GameObject sliderObj = GameObject.Find("LoadingSlider");
        if (sliderObj != null)
        {
            loadingSlider = sliderObj.GetComponent<Slider>();
        }
        AsyncOperation nextSceneOp = SceneManager.LoadSceneAsync(nextSceneName);

        while (!nextSceneOp.isDone)
        {
            if (loadingSlider != null)
            {
                loadingSlider.value = nextSceneOp.progress;
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

    public void ExitScene()
    {
        Application.Quit();
    }

    public void StartGameMenu()
    {
        Debug.Log("게임시작");
        StartSeneTransition("Scene1");
    }
}