using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMonoAwake<AudioManager>
{
    [Header("Audio Source")]
    public AudioSource uiAudioSource;
    public AudioSource GameAudioSource;
    public AudioClip clickSound;
    public AudioClip errorSound;
    public AudioClip Dealcards; //chiabai
    public AudioClip gambling; // danh bai
    public AudioClip addMoney; //cong tien
    public AudioClip deductmoney; // trưtien   

    public float fadeDuration = 2f;
    public bool IsUIAudioOn { get; private set; } = true;
    public bool IsGameAudioOn { get; private set; } = true;
    private Coroutine fadeCoroutine;
    private void Start()
    {
        IsGameAudioOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        IsUIAudioOn = PlayerPrefs.GetInt("UIAudioOn", 1) == 1;

        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckSceneAndPlayAudio();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Enum.TryParse(scene.name, out ESceneName parsedScene))
        {
            SceneFader.Instance.CurrentScene = parsedScene;
        }
        else
        {
            Debug.LogWarning("Scene không xác định: " + scene.name);
            SceneFader.Instance.CurrentScene = ESceneName.Login;
        }
        CheckSceneAndPlayAudio();
    }

    private void CheckSceneAndPlayAudio()
    {
        ESceneName sceneID = SceneFader.Instance.CurrentScene;

        if (sceneID == ESceneName.Login)
        {
            // Tắt nhạc nền
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            if (GameAudioSource.isPlaying)
                fadeCoroutine = StartCoroutine(FadeOutAudio());
        }
        else if (sceneID == ESceneName.Room || sceneID == ESceneName.GamePlay)
        {
            if (IsGameAudioOn && !GameAudioSource.isPlaying)
            {
                GameAudioSource.volume = 0f;
                GameAudioSource.Play();

                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                fadeCoroutine = StartCoroutine(FadeInAudio());
            }
        }
    }
   
    public void Nhacneen()
    {
        GameAudioSource.volume = 0f;
    }

    public void PlayClick()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(clickSound);
    }

    public void PlayError()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(errorSound);
    }
    public void DealCards()             // chiabai
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(Dealcards);
    }
    public void Gambling()           // danhbai
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(gambling);
    }
    public void AddMoneyCoin()       // congtien
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(addMoney);
    }


    public void Deductmoney()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(deductmoney);
    }
    public void SetGameAudio(bool on)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (on && !GameAudioSource.isPlaying)
        {
            GameAudioSource.volume = 0f;
            GameAudioSource.Play();
            IsGameAudioOn = true;
            fadeCoroutine = StartCoroutine(FadeInAudio());
        }
        else if (!on && GameAudioSource.isPlaying)
        {
            fadeCoroutine = StartCoroutine(FadeOutAudio());
        }
    }
    public void SetUIAudio(bool on)
    {
        IsUIAudioOn = on;
    }
    IEnumerator FadeInAudio()
    {
        GameAudioSource.volume = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            GameAudioSource.volume = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        GameAudioSource.volume = 1f; // Đảm bảo max volume sau khi kết thúc
    }

    private IEnumerator FadeOutAudio()
    {
        float startVolume = GameAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            GameAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        GameAudioSource.volume = 0f;
        GameAudioSource.Stop();
        IsGameAudioOn = false;
    }


}
