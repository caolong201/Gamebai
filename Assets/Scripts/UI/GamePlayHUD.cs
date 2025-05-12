using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using UnityEngine;

public class GamePlayHUD : MonoBehaviour
{
    [SerializeField] PhomGameManager phomGameManager;
    [SerializeField] private GameObject btnDanhBai, btnRutBai, btnAnBai, btnChiaBai, btnXepBai, btnHaPhom, btnGuiBai,bntChat;
    [SerializeField] GameObject showChat;
    public RectTransform panelParent;
    float timeShow = 2f;

    private void Start()
    {
        btnChiaBai.SetActive(false);
        ResetUI();
        NetworkManager.Instance.EnterGameRespone.OnDataUpdated += EnterGameRespone;
        NetworkManager.Instance.PlayerLeftRespone.OnDataUpdated += PlayerLeftRespone;

        panelParent.anchoredPosition = new Vector2(-354, 894f);
    }

    private void Update()
    {
        showAnonymous();
    }
    private void OnDestroy()
    {
        NetworkManager.Instance.EnterGameRespone.OnDataUpdated -= EnterGameRespone;
        NetworkManager.Instance.PlayerLeftRespone.OnDataUpdated -= PlayerLeftRespone;
    }

    private void EnterGameRespone(EnterGameRespone obj)
    {
        if (obj.data.position.Count >= 2 && GameManager.Instance.IsRoomMaster())
        {
            btnChiaBai.SetActive(true);
        }
    }
    
    private void PlayerLeftRespone(PlayerLeftRespone obj)
    {
        Debug.Log("PlayerLeft");
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (GameManager.Instance.GetPlayersCount() < 2)
            {
                btnChiaBai.SetActive(false);
            }
        });
    }

    public void ResetUI()
    {
        ShowDanhBai(false);
        ShowRutBai(false);
        ShowAnBai(false);
        ShowXepBai(false);
        ShowHaPhom(false);
        ShowHaPhom(false);
        ShowGuiBai(false);
    }

    public void ShowChiaBai(bool isShow)
    {
        btnChiaBai.SetActive(isShow);
    }
    
    public void ShowXepBai(bool isShow)
    {
        btnXepBai.SetActive(isShow);
    }

    public void ShowDanhBai(bool isShow)
    {
        btnDanhBai.SetActive(isShow);
    }

    public void ShowRutBai(bool isShow)
    {
        btnRutBai.SetActive(isShow);
    }

    public void ShowAnBai(bool isShow)
    {
        btnAnBai.SetActive(isShow);
    }
    
    public void ShowHaPhom(bool isShow)
    {
        btnHaPhom.SetActive(isShow);
    }
    
    public void ShowGuiBai(bool isShow)
    {
        btnGuiBai.SetActive(isShow);
    }

    public void OnbtnDanhBaiClick()
    {
        phomGameManager.OnDiscardCard();
        ShowRutBai(false);
        ShowAnBai(false);
    }

    public void OnbtnRutBaiClick()
    {
        ShowRutBai(false);
        ShowAnBai(false);
        ShowDanhBai(true);

        phomGameManager.OnDrawFromDeck();
        phomGameManager.isCanSelectCard = true;
    }

    public void OnbtnAnBaiClick()
    {
        ShowRutBai(false);
        ShowAnBai(false);
        ShowDanhBai(true);

        phomGameManager.OnDrawFromDiscard();
        phomGameManager.isCanSelectCard = true;
    }

    public void OnbtnChiaBaiClick()
    {
        string json = JsonMapper.ToJson(new BaseWebsocketRequest((int)ENetworkHeader.StartGame));
        NetworkManager.Instance.SendJsonData(json);
        btnChiaBai.SetActive(false);
    }
    
    public void OnbtnHaPhomClick()
    {
        btnHaPhom.SetActive(false);
        phomGameManager.HaPhom();
    }
    
    public void OnbtnGuiBaiClick()
    {
        btnGuiBai.SetActive(false);
        phomGameManager.GuiBai();
    }
    public void ShowChat()
    {
        bntChat.SetActive(false);
        //showChat.SetActive(true);
        panelParent.DOAnchorPos(new Vector2 (-1086f, 894f), 1f).SetEase(Ease.OutCubic);
    }



    public void showAnonymous()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowAnonymousPopup();
        }
#endif
    }

    public void ShowAnonymousPopup()
    {
        UIManager.Instance.ShowDialog(DialogName.UIAnonymous, new AnonymousData(() =>
        {
            Debug.Log("OK Clicked from Anonymous");
        }));

     
        DOVirtual.DelayedCall(timeShow, () =>
        {
            UIManager.Instance.HideDialog(DialogName.UIAnonymous);
        });
    }
}
