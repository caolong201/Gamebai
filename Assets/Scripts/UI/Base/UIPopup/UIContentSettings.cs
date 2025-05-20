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

        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        sliderNhacNen.SetValueWithoutNotify(isMusicOn ? 0f : 1f); 

        sliderAmThanhUI.onValueChanged.RemoveAllListeners();
        sliderAmThanhUI.onValueChanged.AddListener(OnSliderAmThanhUIChanged);

        bool isUIAudioOn = PlayerPrefs.GetInt("UIAudioOn", 1) == 1;
        sliderAmThanhUI.SetValueWithoutNotify(isUIAudioOn ? 0f : 1f);
    }
    public void OnSliderNhacNenChanged(float value)
    {
        bool isOn = value <= 0.5f;
        AudioManager.Instance.SetGameAudio(isOn);
        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void OnSliderAmThanhUIChanged(float value)
    {
        bool isOn = value <= 0.5f;
        AudioManager.Instance.SetUIAudio(isOn);
        PlayerPrefs.SetInt("UIAudioOn", isOn ? 1 : 0);
        PlayerPrefs.Save(); 
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

