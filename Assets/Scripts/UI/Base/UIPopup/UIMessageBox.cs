using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIMessageBox : GUIBaseDialogHandler
{

	[SerializeField] private TextMeshProUGUI txtContent;
	[SerializeField] private GameObject btnClose;
    [SerializeField] private GameObject btnOK;
    [SerializeField] private TextMeshProUGUI btnOkText;
    private Action OnOkClick;
    private Action OnCloseClick;

    [SerializeField] private Color colNotice, colWarning;
    [SerializeField] private Sprite btnBgGreen, btnBgPink;

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        MessageBoxData data = (MessageBoxData)parameter;
        txtContent.text = data.mContent;
        btnClose.SetActive(data.mIsShowBtnClose);

        btnOkText.text = data.mBtnOkText;

        OnOkClick = data.onOkClick;
        OnCloseClick = data.onCloseClick;

        
    }

	public void OnbtnOkClicked()
	{
        OnOkClick?.Invoke();
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
    public bool mIsWarningPopup = false;
    public bool mIsRemoveAdPopup = false;

    public MessageBoxData(string content, bool isShowBtnCancel, string _mBtnOkText = "ok", Action _onOkClick = null,  Action _onCloseClick = null, bool _isWarningPopup = false)
	{
		mContent = content;
		mIsShowBtnClose = isShowBtnCancel;
        mBtnOkText = _mBtnOkText;

        onOkClick = _onOkClick;
        onCloseClick = _onCloseClick;
        mIsWarningPopup = _isWarningPopup;
    }

    public MessageBoxData(string content,  string _mBtnOkText = "ok", Action _onOkClick = null, Action _onCloseClick = null, bool _isRemoveAdPopup = false)
    {
        mContent = content;
        mIsShowBtnClose = true;
        mBtnOkText = _mBtnOkText;

        onOkClick = _onOkClick;
        onCloseClick = _onCloseClick;
        mIsWarningPopup = false;
        mIsRemoveAdPopup = _isRemoveAdPopup;
    }
}
