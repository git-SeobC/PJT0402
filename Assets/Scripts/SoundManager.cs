using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public enum BGMType
{
    MenuBGM,
    Scene1StrangeFarmBGM,
    Scene2CaveBGM,
    Scene2BGM,
    EndingCreditsBGM
}

public enum SFXType
{
    EnemyDamagedSFX,
    PlayerHitSFX,
    PlayerJumpSFX,
    PlayerStepSFX,
    PickupItemSFX,
    MenuOpenSFX,
    MenuSelectSFX,
    MenuClickSFX,
    LandingSFX,
    SpikeTrapSFX,
    BladeDieSFX,
    DefaultDieSFX,
    OrcHitSFX,
    OrcDeathSFX,
    BrickBreakSFX,
    ItemLifeSFX,
    GameClearSoundSFX,
    GameOverSoundSFX,
    GameOverSound2SFX,
    LoadingStartSFX,
    LoadingFinishSFX,
    TypingSFX,
    IntoPortalSFX
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource; // 배경음 재생용 오디오소스
    public AudioSource sfxSource; // 효과음 재생용 오디오소스

    public Dictionary<BGMType, AudioClip> bgmDic = new Dictionary<BGMType, AudioClip>();
    public Dictionary<SFXType, AudioClip> sfxDic = new Dictionary<SFXType, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // 게임 시작 시 자동으로 실행되는 초기화 함수
    // 게임 신 로딩 전에 로고 나올 때 먼저 실행 시킴
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitSoundManager()
    {
        GameObject obj = new GameObject("SoundManager");
        Instance = obj.AddComponent<SoundManager>();
        DontDestroyOnLoad(obj);

        // BGM 설정
        GameObject bgmObj = new GameObject("BGM");
        SoundManager.Instance.bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmObj.transform.SetParent(obj.transform);
        SoundManager.Instance.bgmSource.loop = true;
        SoundManager.Instance.bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);

        // SFX 설정
        GameObject sfxObj = new GameObject("SFX");
        SoundManager.Instance.sfxSource = sfxObj.AddComponent<AudioSource>();
        SoundManager.Instance.sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        sfxObj.transform.SetParent(obj.transform);

        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        // 리소스 파일에 있는 BGM 파일을 모두 로드함 -> 파일이 클 수록 느려 게임 로딩이 길어 진다는 단점이 있음

        foreach (var clip in bgmClips)
        {
            try
            {
                BGMType type = (BGMType)Enum.Parse(typeof(BGMType), clip.name);
                SoundManager.Instance.bgmDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("BGM Enum 필요 : " + clip.name);
            }
        }

        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sound/SFX");
        foreach (var clip in sfxClips)
        {
            try
            {
                SFXType type = (SFXType)Enum.Parse(typeof(SFXType), clip.name);
                SoundManager.Instance.sfxDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("SFX Enum 필요 : " + clip.name);
            }
        }
        SceneManager.sceneLoaded += SoundManager.Instance.OnSceneLoadCompleted;
    }

    public void OnSceneLoadCompleted(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene1")
        {
            SetBGMVolume(0.5f);
            PlayBGM(BGMType.Scene1StrangeFarmBGM, 1f);
        }
        else if (scene.name == "Boss")
        {
            PlayBGM(BGMType.Scene1StrangeFarmBGM, 1f);
        }
    }

    public void PlaySFX(SFXType type)
    {
        sfxSource.PlayOneShot(sfxDic[type]);
    }

    public void PlayBGM(BGMType type, float fadeTime)
    {
        if (bgmSource.clip != null)
        {
            if (bgmSource.clip.name == type.ToString())
            {
                return;
            }
            if (fadeTime == 0)
            {
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
            }
            else
            {
                StartCoroutine(FadeOutBGM (() =>
                {
                    bgmSource.clip = bgmDic[type];
                    bgmSource.Play();
                    StartCoroutine(FadeInBGM(fadeTime));
                }, fadeTime));
            }
        }
        else
        {
            if (fadeTime == 0)
            {
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
            }
            else
            {
                bgmSource.volume = 0;
                bgmSource.clip = bgmDic[type];
                bgmSource.Play();
                StartCoroutine(FadeInBGM(fadeTime));
            }
        }
    }

    public void StopBGM(float fadeTime = 1.0f)
    {
        if (bgmSource.isPlaying)
        {
            if (fadeTime <= 0)
            {
                bgmSource.Stop();
                bgmSource.clip = null;
            }
            else
            {
                StartCoroutine(FadeOutBGM(() =>
                {
                    bgmSource.Stop();
                    bgmSource.clip = null;
                }, fadeTime));
            }
        }
    }

    private IEnumerator FadeOutBGM(Action onComplete, float duration)
    {
        float startVolume = bgmSource.volume;
        float time = 0;

        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        onComplete?.Invoke();
    }

    private IEnumerator FadeInBGM(float duration = 1.0f)
    {
        float targetVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        float time = 0f;

        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVoluime(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}

