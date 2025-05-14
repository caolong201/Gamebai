using BestHTTP.JSON.LitJson;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class UICreateTable : GUIBaseDialogHandler
{
    [SerializeField] private Button buttonMinus;
    [SerializeField] private Button buttonPlus;
    [SerializeField] GameObject imgTaoban;
    private int[] betLevels = { 100, 500, 1000 }; // Các mức cược
    private Action onCloseClick;
    public TMP_InputField ipPassword;
   
    [SerializeField] SimpleScrollSnap scrollSnap;

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        imgTaoban.SetActive(true);
        ipPassword.text = "";

        RoomCreatorData data = parameter as RoomCreatorData;
        if (data != null)
        {
            onCloseClick = data.onCloseClick;
        }
        ipPassword.text = "";
     
    }

    public int GetSelectedBet(int index)
    {
        return betLevels[index];
    }
    public void OnBtnCloseClicked()
    {
        onCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UICreateTable);
    }
    public void OnBtnOkClicked()
    {
        string err = String.Empty;
        if (string.IsNullOrEmpty(ipPassword.text))
            err = "Mật khẩu bắt buộc";

        if (!string.IsNullOrEmpty(err))
        {
            UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(err));
            return;
        }

        int betAmount = GetSelectedBet(scrollSnap.SelectedPanel);
        string json = JsonMapper.ToJson(new CreatePrivateTableModel((int)ENetworkHeader.CreatePrivateTable, new CreatePrivateTableModelData()

        {
            password = ipPassword.text.Trim(),
            betAmount = betAmount,
            playerCount = 4

        }));
        
        NetworkManager.Instance.SendJsonData(json);
        UIManager.Instance.HideDialog(DialogName.UICreateTable);
    }

}


public class RoomCreatorData
{
    public Action onCloseClick;
    public RoomCreatorData(Action _onOkClick = null)
    {
        onCloseClick = _onOkClick;
    }
    public RoomCreatorData(Action _onOkClick, Action _onCance)
    {
        onCloseClick = _onOkClick;
    }
}


