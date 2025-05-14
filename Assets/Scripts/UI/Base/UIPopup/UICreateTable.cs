using BestHTTP.JSON.LitJson;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private int currentBetIndex = 0;
    private Action onCloseClick;
    public TextMeshProUGUI[] texts; 
    public TMP_InputField ipPassword;
    //[SerializeField] private TMP_InputField inputField; 
    //[SerializeField] private TextMeshProUGUI placeholderText; 
    private int normalFontSize = 45;
    private int selectedFontSize = 60;
    public override void OnStart()
    { 
        base.OnStart();
        imgTaoban.SetActive(false);
        ipPassword.text = "";

        //if (inputField != null && placeholderText != null)
        //{
        //    // Đặt kích thước font ban đầu cho placeholder
        //    //placeholderText.fontSize = normalFontSize;

        //    //inputField.onSelect.AddListener(OnInputFieldSelect);
        //    //inputField.onDeselect.AddListener(OnInputFieldDeselect);
        //}
    }

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        imgTaoban.SetActive(true);

        RoomCreatorData data = parameter as RoomCreatorData;
        if (data != null)
        {
            onCloseClick = data.onCloseClick;
        }
        ipPassword.text = "";
     
    }
    public override void OnEndHide(bool isDestroy)
    {
        base.OnEndHide(isDestroy);
        imgTaoban.SetActive(false);
    }

    public int GetSelectedBet()
    {
        return betLevels[currentBetIndex];
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
            UIManager.Instance.GetDialog(DialogName.UIMessageBox)?.transform.SetAsLastSibling();
            return;
        }
        foreach (var text in texts)
        {
            RectTransform itemRect = text.GetComponent<RectTransform>();
            Debug.Log("Đã chọn: " + text.text);
            break;
        }
        int betAmount = GetSelectedBet();
        int playerCount = 4;
        string json = JsonMapper.ToJson(new CreatePrivateTableModel((int)ENetworkHeader.CreatePrivateTable, new CreatePrivateTableModelData()

        {
            password = ipPassword.text.Trim(),
            betAmount = betAmount,
            playerCount = playerCount

        }));
        Debug.Log("JSON gửi đi: " + json);
        NetworkManager.Instance.SendJsonData(json);
        UIManager.Instance.HideDialog(DialogName.UICreateTable);
    }

    //private void OnInputFieldSelect(string text)
    //{
    //    // Thay đổi kích thước font của placeholder khi chọn InputField
    //    placeholderText.fontSize = selectedFontSize;
    //}

    //// Khi InputField bị bỏ chọn (khi mất focus)
    //private void OnInputFieldDeselect(string text)
    //{
    //    placeholderText.fontSize = normalFontSize;
    //}
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


