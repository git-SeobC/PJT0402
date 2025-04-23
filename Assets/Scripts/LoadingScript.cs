using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public Slider loadingSlider;
    public RectTransform handleTransform;
    public float loadingTime = 2.0f;
    public string nextSceneName = "Scene1";
    public GameObject handle;
    private Image handlerImage;
    private Sprite[] images;
    private float animationFrameRate = 10f; // 1초에 10프레임
    private int currentFrame = 0;
    private float animationTimer = 0f;

    private float timer = 0f;

    void Start()
    {
        loadingSlider.value = 0f;
        handlerImage = handle.GetComponent<Image>();
        images = Resources.LoadAll<Sprite>("loadingFinn");
        SoundManager.Instance.PlaySFX(SFXType.LoadingStartSFX);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / loadingTime);

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

        //UpdateHandlePosition(progress);

        if (progress >= 1f)
        {
            SoundManager.Instance.PlaySFX(SFXType.LoadingFinishSFX);
            SceneManager.LoadScene(nextSceneName);
        }
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