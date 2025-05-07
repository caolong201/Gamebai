
using BestHTTP.JSON.LitJson;
using Suni.Enum;
using Suni.Network;
using UnityEngine;
using TMPro;

public class RomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI textperson;

    private RoomTableInfo mData;

    public void Init(RoomTableInfo data)
    {
        mData = data;
        if (priceText != null)
        {
            priceText.text = data.betAmount.FormatCoins();
        }

        if (textperson != null)
        {
            textperson.text = data.clientCount.ToString();
        }
    }

    public void OnItemClicked()
    {
        Debug.Log("Bet amount: " + mData.betAmount);
        string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, mData.betAmount));
        NetworkManager.Instance.SendJsonData(json);
    }
}