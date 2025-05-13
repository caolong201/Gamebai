using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UICEntertable : GUIBaseDialogHandler
{
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject btnCancel;
    private Action OnCloseClick;
    public override void OnStart()
    {
        base.OnStart();
        black.SetActive(false);
    }
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        black.SetActive(true);
        EntertableData data = (EntertableData)parameter;
        OnCloseClick = data.onCloseClick;
    }

    public override void OnEndHide(bool isDestroy)
    {
        base.OnEndHide(isDestroy);
        black.SetActive(false);
    }
    public void OnbtnCloseClicked()
    {
        OnCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIEntertable);
    }
}
public class EntertableData
{
    public Action onCloseClick;
    public EntertableData(Action _onOkClick = null)
    {
        onCloseClick = _onOkClick;
    }
    public EntertableData(Action _onOkClick, Action _onCance)
    {
        onCloseClick = _onOkClick;
    }
}
