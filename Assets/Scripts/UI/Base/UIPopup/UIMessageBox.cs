using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIMessageBox : GUIBaseDialogHandler
{
    [SerializeField] private GameObject black;
    [SerializeField] private TextMeshProUGUI txtContent;

    [SerializeField] private GameObject btnOK;
    [SerializeField] private TextMeshProUGUI btnOkText;

    [SerializeField] private GameObject btnCancel;
    [SerializeField] private TextMeshProUGUI btnCancelText;

    private Action OnOkClick;
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
        
        MessageBoxData data = (MessageBoxData)parameter;
        UpdateTextContent(data.mContent);
        btnCancel.SetActive(data.mIsShowBtnClose);

        btnOkText.text = data.mBtnOkText;

        OnOkClick = data.onOkClick;
        OnCloseClick = data.onCloseClick;
    }

    public override void OnEndHide(bool isDestroy)
    {
        base.OnEndHide(isDestroy);
        black.SetActive(false);
    }

    public void OnbtnOkClicked()
    {
        OnOkClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIMessageBox);
    }

    public void OnbtnCloseClicked()
    {
        OnCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIMessageBox);
    }


    public void UpdateTextContent(string text)
    {
        txtContent.text = text;
    }
}

public class MessageBoxData
{
    public string mContent = "";
    public bool mIsShowBtnClose;
    public string mBtnOkText = "";
    public string mBtnCancelText = "";
    public Action onOkClick;
    public Action onCloseClick;

    public MessageBoxData(string content, string _mBtnOkText = "OK", Action _onOkClick = null)
    {
        mContent = content;
        mIsShowBtnClose = false;
        mBtnOkText = _mBtnOkText;
        onOkClick = _onOkClick;
    }

    public MessageBoxData(string content, string _mBtnOkText, string _mBtnCancelText, Action _onOkClick = null,
        Action _onCancelClick = null)
    {
        mContent = content;
        mIsShowBtnClose = true;
        mBtnOkText = _mBtnOkText;
        mBtnCancelText = _mBtnCancelText;
        onOkClick = _onOkClick;
        onCloseClick = _onCancelClick;
    }
}