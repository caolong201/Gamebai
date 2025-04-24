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
    [SerializeField] private GameObject btnDanhBai, btnRutBai, btnAnBai, btnChiaBai, btnXepBai, btnHaPhom;

    private void Start()
    {
        btnChiaBai.SetActive(false);
        NetworkManager.Instance.EnterGameRespone.OnDataUpdated += EnterGameRespone;
        NetworkManager.Instance.PlayerLeftRespone.OnDataUpdated += PlayerLeftRespone;
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

    public void OnbtnDanhBaiClick()
    {
        phomGameManager.OnDiscardCard();
        ShowDanhBai(false);
        ShowRutBai(false);
        ShowAnBai(false);
        phomGameManager.isCanSelectCard = false;
    }

    public void OnbtnRutBaiClick()
    {
        ShowRutBai(false);
        ShowAnBai(false);

        phomGameManager.OnDrawFromDeck();
        phomGameManager.isCanSelectCard = true;
    }

    public void OnbtnAnBaiClick()
    {
        ShowRutBai(false);
        ShowAnBai(false);

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
}