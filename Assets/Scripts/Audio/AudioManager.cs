using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMonoAwake<AudioManager>
{
    [Header("Audio Source")]
    public AudioSource uiAudioSource;
    public AudioSource GameAudioSource;   //nhac nen
    public AudioClip clickSound;
    public AudioClip errorSound;
    public AudioClip Dealcards;           //chiabai
    public AudioClip gambling;           // danh bai
    public AudioClip addMoney;           //cong tien
    public AudioClip chat;
    public AudioClip deductmoney;       // trưtien  
    public AudioClip flipCard;
    public AudioClip winClip;
    public AudioClip momClip;
    public float fadeDuration = 2f;
    public bool IsUIAudioOn { get; private set; } = true;
    public bool IsGameAudioOn { get; private set; } = true;
    // xử lý âm thanh theo trình tự
    //private Queue<AudioClip> moneySoundQueue = new Queue<AudioClip>();
    private bool isPlayingMoneySound = false;
    private bool isPlayingWinClip = false;
    private enum MoneySoundType { Add, Deduct }
    private Queue<(MoneySoundType type, AudioClip clip)> moneySoundQueue = new Queue<(MoneySoundType, AudioClip)>();
    private MoneySoundType? currentMoneySoundType = null;
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
            GameAudioSource.DOKill();
            GameAudioSource.DOFade(0f, 2f).OnComplete(() => GameAudioSource.Stop());
        }
        else if (sceneID == ESceneName.Room || sceneID == ESceneName.GamePlay)
        {
            if (IsGameAudioOn && !GameAudioSource.isPlaying)
            {
                GameAudioSource.volume = 0f;
                GameAudioSource.Play();
                GameAudioSource.DOFade(1, 2f);
            }
        }
    }
    public void FlipCard()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(flipCard);
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
        if (!IsUIAudioOn || deductmoney == null) return;
        EnqueueMoneySound(MoneySoundType.Add, addMoney);
    }
    public void Deductmoney()      // trừ tiền
    {
        if (!IsUIAudioOn || deductmoney == null) return;
        EnqueueMoneySound(MoneySoundType.Deduct, deductmoney);
    }

    private void EnqueueMoneySound(MoneySoundType type, AudioClip clip)
    {

        if (isPlayingMoneySound && currentMoneySoundType != type)
            return;

        moneySoundQueue.Enqueue((type, clip));
        if (!isPlayingMoneySound)
        {
            StartCoroutine(PlayMoneySoundQueue());
        }
    }
    private IEnumerator PlayMoneySoundQueue()
    {
        isPlayingMoneySound = true;

        if (moneySoundQueue.Count > 0)
            currentMoneySoundType = moneySoundQueue.Peek().type;

        while (moneySoundQueue.Count > 0)
        {
            var item = moneySoundQueue.Dequeue();
            uiAudioSource.clip = item.clip;
            uiAudioSource.Play();

            yield return new WaitForSeconds(item.clip.length + 0.05f);
        }

        currentMoneySoundType = null;
        isPlayingMoneySound = false;
    }
    public void Chatsound()     // Chatsound
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(chat);
    }
    public void WinClip()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(winClip);
    }
    public void MomClip()
    {
        if (!IsUIAudioOn) return;
        uiAudioSource.PlayOneShot(momClip);
    }
    public void SetGameAudio(bool on)
    {
        GameAudioSource.DOKill();

        if (on && !GameAudioSource.isPlaying)
        {
            GameAudioSource.volume = 0f;
            GameAudioSource.Play();
            IsGameAudioOn = true;
            GameAudioSource.DOFade(1f, fadeDuration);
        }
        else if (!on && GameAudioSource.isPlaying)
        {
            GameAudioSource.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                GameAudioSource.Stop();
                IsGameAudioOn = false;
            });
        }
    }
    public void SetUIAudio(bool on)
    {
        IsUIAudioOn = on;
    }

}
