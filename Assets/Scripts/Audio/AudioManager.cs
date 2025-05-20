using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoAwake<AudioManager>
{
    [Header("Audio Source")]
    public AudioSource uiAudioSource;
    public AudioSource GameAudioSource;
    public AudioClip clickSound;
    public AudioClip errorSound;
    public AudioClip Dealcards;
    public AudioClip gambling;
   

    public float fadeDuration = 2f;
    public bool IsUIAudioOn { get; private set; } = true;
    public bool IsGameAudioOn { get; private set; } = true;
    private Coroutine fadeCoroutine;
    private void Start()
    {
        
        IsGameAudioOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        IsUIAudioOn = PlayerPrefs.GetInt("UIAudioOn", 1) == 1;

        if (GameAudioSource.clip != null)
        {
            GameAudioSource.volume = 0f;
            if (IsGameAudioOn)
            {
                GameAudioSource.Play();
                fadeCoroutine = StartCoroutine(FadeInAudio());
            }
        }
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
        GameAudioSource.volume = 5f; // Đảm bảo max volume sau khi kết thúc
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
