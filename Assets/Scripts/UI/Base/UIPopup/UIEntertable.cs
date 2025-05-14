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
    [SerializeField] private GameObject btnCancel;
    public TMP_InputField ipUsername;
    public TMP_InputField ipPassword;
    private Action OnCloseClick;
    private Action OnoklClick;
   
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        EntertableData data = (EntertableData)parameter;
        OnCloseClick = data.onCloseClick;
        ipUsername.text = "";
        ipPassword.text = "";
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
            err = "Vui lòng nhập số bàn.";
        if (string.IsNullOrEmpty(ipPassword.text))
            err = "Vui lòng nhập mật khẩu.";
        
        if (!string.IsNullOrEmpty(err))
        {
            UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(err));
            return;
        }

        string json = JsonMapper.ToJson(new JoinPrivateTableModel((int)ENetworkHeader.JoinPrivateTable, new JoinPrivateTableModelData()
        {
            tableId = ipUsername.text.Trim(),
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
