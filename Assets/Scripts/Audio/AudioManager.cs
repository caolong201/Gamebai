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
    public AudioClip loseClipp;
    public AudioClip momClip;
    public float fadeDuration = 2f;
    public bool IsUIAudioOn { get; private set; } = true;
    public bool IsGameAudioOn { get; private set; } = true;
    private bool isPlayingMoneySound = false;
    private enum MoneySoundType { Add, Deduct }
    private Queue<(MoneySoundType type, AudioClip clip)> moneySoundQueue = new Queue<(MoneySoundType, AudioClip)>();
    private MoneySoundType? currentMoneySoundType = null;

    public enum UIAudioType
    {
        Click,
        Error,
        DealCards,
        Gambling,
        AddMoney,
        DeductMoney,
        Chat,
        FlipCard,
        Win,
        Lose,
        Mom
    }
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
    public void PlayUIAudio(UIAudioType type)
    {
        if (!IsUIAudioOn) return;

        AudioClip clipToPlay = null;

        switch (type)
        {
            case UIAudioType.Click:
                clipToPlay = clickSound;
                break;
            case UIAudioType.Error:
                clipToPlay = errorSound;
                break;
            case UIAudioType.DealCards:
                clipToPlay = Dealcards;
                break;
            case UIAudioType.Gambling:
                clipToPlay = gambling;
                break;
            case UIAudioType.AddMoney:
                EnqueueMoneySound(MoneySoundType.Add, addMoney);
                return; 
            case UIAudioType.DeductMoney:
                EnqueueMoneySound(MoneySoundType.Deduct, deductmoney);
                return; 
            case UIAudioType.Chat:
                clipToPlay = chat;
                break;
            case UIAudioType.FlipCard:
                clipToPlay = flipCard;
                break;
            case UIAudioType.Win:
                clipToPlay = winClip;
                break;
            case UIAudioType.Lose:
                clipToPlay = loseClipp;
                break;
            case UIAudioType.Mom:
                clipToPlay = momClip;
                break;
            default:
                Debug.LogWarning("UIAudioType không hợp lệ: " + type);
                return;
        }

        if (clipToPlay != null)
        {
            uiAudioSource.PlayOneShot(clipToPlay);
        }
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
