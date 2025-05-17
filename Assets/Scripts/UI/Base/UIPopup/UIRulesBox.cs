using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;

public class UIRulesBox : GUIBaseDialogHandler
{

    public float collapsedHeight = 200f;
    public float expandedHeight = 500f;
    public float animationDuration = 0.3f;

    private Coroutine currentAnim;
    public RectTransform scrollView;
    private Action OnCloseClick;

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);

        UIRulesBoxData data = (UIRulesBoxData)parameter;
 
        OnCloseClick = data.onCloseClick;
    }

    public void OnbtnCloseClicked()
    {
        OnCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIRulesBox);
    }

    public class UIRulesBoxData
    {
        public Action onCloseClick;

        public UIRulesBoxData(Action _onOkClick = null)
        {
            onCloseClick = _onOkClick;

        }
        public UIRulesBoxData(Action _onOkClick, Action _onCance)
        {
            onCloseClick = _onOkClick;

        }
    }
}
