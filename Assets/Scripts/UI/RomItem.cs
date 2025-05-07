
using BestHTTP.JSON.LitJson;
using Suni.Enum;
using Suni.Network;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class RomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI textperson;

    private RoomTableInfo mData;
    private Action<RoomTableInfo> onClickedCallback;
    public void Init(RoomTableInfo data, Action<RoomTableInfo> onClicked)
    {
        mData = data;
        onClickedCallback = onClicked;
        if (priceText != null)
        {
            priceText.text = data.betAmount.FormatCoins();
        }

        if (textperson != null)
        {
            textperson.text = data.clientCount.ToString();
        }
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners(); 
            btn.onClick.AddListener(OnItemClicked);
        }

    }

    public void OnItemClicked()
    {
        onClickedCallback?.Invoke(mData);
    }
}


