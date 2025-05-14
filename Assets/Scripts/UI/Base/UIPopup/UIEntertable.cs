using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using BestHTTP.JSON.LitJson;
using Suni.Enum;
using Suni.Network;

public class UICEntertable : GUIBaseDialogHandler
{
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject btnCancel;
    public TMP_InputField ipUsername;
    public TMP_InputField ipPassword;
    private Action OnCloseClick;
    private Action OnoklClick;
    public override void OnStart()
    {
        base.OnStart();
        black.SetActive(false);
        ipUsername.text = "";
        ipPassword.text = "";
    }
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        black.SetActive(true);
        EntertableData data = (EntertableData)parameter;
        OnCloseClick = data.onCloseClick;
        ipUsername.text = "";
        ipPassword.text = "";
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
    public void OnbtOkClick()
    {
        OnoklClick?.Invoke();
       
        string err = String.Empty;
        if (string.IsNullOrEmpty(ipUsername.text))
            err = "Mật khẩu bắt buộc";
        if (string.IsNullOrEmpty(ipPassword.text))
            err = "Mật khẩu bắt buộc";
        Debug.Log("ipUsername: " + ipUsername.text + ", ipPassword: " + ipPassword.text);
        if (!string.IsNullOrEmpty(err))
        {
            UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(err));
            UIManager.Instance.GetDialog(DialogName.UIMessageBox)?.transform.SetAsLastSibling();
            return;
        }

        string json = JsonMapper.ToJson(new JoinPrivateTableModel((int)ENetworkHeader.JoinPrivateTable, new JoinPrivateTableModelData()
        {
            username = ipUsername.text.Trim(),
            password = ipPassword.text.Trim(),

        }));

        Debug.Log(" JSON gửi đi: " + json);
        NetworkManager.Instance.SendJsonData(json);
        UIManager.Instance.HideDialog(DialogName.UIEntertable);
    }
}
public class EntertableData
{
    public Action onCloseClick;
    public Action onOkClick;
    public EntertableData(Action _onOkClick = null)
    {
        onCloseClick = _onOkClick;
        onOkClick = _onOkClick;
    }
    public EntertableData(Action _onOkClick, Action _onCance)
    {
        onCloseClick = _onOkClick;
        onOkClick = _onOkClick;
    }
}
