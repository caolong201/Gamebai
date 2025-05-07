
using System;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomHUD : MonoBehaviour
{
    [SerializeField] private Image imgAvatar;
    [SerializeField] private TextMeshProUGUI txtCoin, txtNickname;
    [SerializeField] RectTransform header, footer;
    [SerializeField] GameObject Phom;

    private void Start()
    {
        LoadAvatar();
        txtCoin.text = ((int)GameManager.Instance.Coin).FormatCoins();
        txtNickname.text = GameManager.Instance.NickName;

        header.anchoredPosition = new Vector2(0, 150);
        header.DOAnchorPosY(-107, .8f).SetEase(Ease.OutCirc);

        footer.anchoredPosition = new Vector2(0, -150);
        footer.DOAnchorPosY(119, .8f).SetEase(Ease.OutCirc);

    }

    private void LoadAvatar()
    {
        string avatarName = "avatar_" + (string.IsNullOrEmpty(GameManager.Instance.AvatarUrl) ? "0" : GameManager.Instance.AvatarUrl);
        // Try loading the avatar
        Sprite loadedAvatar = Resources.Load<Sprite>("Avatar/" + avatarName);

        if (loadedAvatar != null)
        {
            imgAvatar.sprite = loadedAvatar;
        }
        else
        {
            Debug.LogError("Failed to load avatar: " + avatarName);
        }
    }

    public void OnbtnPhomClicked()
    {
        //string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, 100));
        //NetworkManager.Instance.SendJsonData(json);
        Phom.SetActive(true);
       
    }
}
