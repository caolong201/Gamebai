using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFx : MonoBehaviour
{
    public enum SoundType { Click, Error, Dealcards, Gambling, flipCard, Chatsound }
    [Header("Loại âm thanh cho button")]
    public SoundType soundType = SoundType.Click;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlaySound);
    }
    void PlaySound()
    {
        if (AudioManager.Instance == null) return;

        switch (soundType)
        {
            case SoundType.Click:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.Click);
                break;
            case SoundType.Error:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.Error);
                break;
            case SoundType.Dealcards:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.DealCards);
                break;
            case SoundType.Gambling:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.Gambling);
                break;
            case SoundType.flipCard:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.FlipCard);
                break;
            case SoundType.Chatsound:
                AudioManager.Instance.PlayUIAudio(AudioManager.UIAudioType.Chat);
                break;
        }
    }
}
    
