using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFx : MonoBehaviour
{
    public enum SoundType { Click, Error }
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
                AudioManager.Instance.PlayClick();
                break;
            case SoundType.Error:
                AudioManager.Instance.PlayError();
                break;
        }
    }
}
    
