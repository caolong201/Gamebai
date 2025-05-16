using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIContentSettings : GUIBaseDialogHandler
{   
    private Action OnCloseClick;
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        UIContentSettingsData data = (UIContentSettingsData)parameter;
        OnCloseClick = data.onCloseClick;
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

