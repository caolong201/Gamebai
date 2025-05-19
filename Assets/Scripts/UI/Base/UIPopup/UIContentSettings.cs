using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIContentSettings : GUIBaseDialogHandler
{   
    private Action OnCloseClick;
    public Slider sliderNhacNen;
    public Slider sliderAmThanhUI;

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        UIContentSettingsData data = (UIContentSettingsData)parameter;
        OnCloseClick = data.onCloseClick;
        sliderNhacNen.onValueChanged.RemoveAllListeners();
        sliderNhacNen.onValueChanged.AddListener(OnSliderNhacNenChanged);
        sliderNhacNen.SetValueWithoutNotify(AudioManager.Instance.GameAudioSource.isPlaying ? 0f : 1f);

        sliderAmThanhUI.onValueChanged.RemoveAllListeners();
        sliderAmThanhUI.onValueChanged.AddListener(OnSliderAmThanhUIChanged);     
        sliderAmThanhUI.SetValueWithoutNotify(AudioManager.Instance.IsUIAudioOn ? 0f : 1f);
    }
    public void OnSliderNhacNenChanged(float value)
    {
        bool isOn = value <= 0.5f;
        AudioManager.Instance.SetGameAudio(isOn);
    }
    public void OnSliderAmThanhUIChanged(float value)
    {
        // value thấp (gần 0) = ON, value cao (gần 1) = OFF
        bool isOn = value <= 0.5f;
        AudioManager.Instance.SetUIAudio(isOn);
    }
    public void OnbtnCloseClicked()
    {
        OnCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIContentSettings);
    }

}

public class UIContentSettingsData
{
    public Action onCloseClick;

    public UIContentSettingsData(Action _onOkClick = null)
    {
        onCloseClick = _onOkClick;
     
    }
    public UIContentSettingsData(Action _onOkClick, Action _onCance)
    {
        onCloseClick = _onOkClick;
   
    }
}

