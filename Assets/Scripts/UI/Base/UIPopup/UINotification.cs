using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UINotification : GUIBaseDialogHandler
{
    [SerializeField] private TextMeshProUGUI txtContent;
    [SerializeField] private RectTransform panelTransform;
  
    Tween tween = null;
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        
        if(tween != null) return;
        
        panelTransform.anchoredPosition = new Vector2(0, 1145f);
        panelTransform.DOAnchorPosY(878f, 0.9f).SetEase(Ease.OutBack);

        UpdateTextContent(parameter as string);
        
        tween = DOVirtual.DelayedCall(2, () =>
        {
            UIManager.Instance.HideDialog(DialogName.UINotification);
            tween = null;
        });
    }

    public void UpdateTextContent(string text)
    {
        txtContent.text = text;
    }
}



