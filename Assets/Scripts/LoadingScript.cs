using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public Slider loadingSlider;
    public RectTransform handleTransform;
    public float loadingTime = 2.0f;
    public string nextSceneName = "Scene1";

    private float timer = 0f;

    void Start()
    {
        loadingSlider.value = 0f;
    }

    void Update()
    {
        //timer += Time.deltaTime;
        //float progress = Mathf.Clamp01(timer / loadingTime);
        //loadingSlider.value = progress;

        //UpdateHandlePosition(progress);

        //if (progress >= 1f)
        //{
        //    SceneManager.LoadScene(nextSceneName);
        //}
    }

    void UpdateHandlePosition(float progress)
    {
        RectTransform fillRect = loadingSlider.fillRect;
        float minX = fillRect.rect.xMin;
        float maxX = fillRect.rect.xMax;
        float width = maxX - minX;

        Vector2 handlePos = handleTransform.anchoredPosition;
        handlePos.x = minX + width * progress;
        handleTransform.anchoredPosition = handlePos;
    }
}