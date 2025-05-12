using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnonymous : GUIBaseDialogHandler
{
    [SerializeField] private TextMeshProUGUI txtContent;
    [SerializeField] private GameObject btnOK; 
    [SerializeField] private RectTransform panelTransform;
    private Action OnOkClick;
    public override void OnStart()
    {
        base.OnStart();
    }
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        panelTransform.anchoredPosition = new Vector2(0, 1145f);
        panelTransform.DOAnchorPosY(878f, 0.9f).SetEase(Ease.OutBack);

        AnonymousData data = (AnonymousData)parameter;
        OnOkClick = data.onOkClick;
    }

    public override void OnEndHide(bool isDestroy)
    {
        base.OnEndHide(isDestroy);  
    }
    public void OnbtnOkClicked()
    {
        OnOkClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIAnonymous);    
    }
    public void UpdateTextContent(string text)
    {
        txtContent.text = text;
    }
}

public class AnonymousData
{
    public Action onOkClick;
    public AnonymousData(Action _onOkClick = null)
    {
        onOkClick = _onOkClick;
    }
    public AnonymousData(Action _onOkClick, Action _onCance)
    {
        onOkClick = _onOkClick;
    }
}


