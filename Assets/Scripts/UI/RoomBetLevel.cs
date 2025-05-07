using System;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RoomBetLevel : MonoBehaviour
{
    public GameObject btnPrefab;
    public Transform content;
    [SerializeField] RectTransform footer;

    private List<RoomTableInfo> roomDatas;
    void Start()
    {
        roomDatas = NetworkManager.Instance.JoinPhomGame.Value.data.roomList;
        
        footer.anchoredPosition = new Vector2(0, -150);
        footer.DOAnchorPosY(119, .8f).SetEase(Ease.OutCirc);

        for (int i = 0; i < roomDatas.Count; i++)
        {
            GameObject btnObj = Instantiate(btnPrefab, content);
            RomItem bet = btnObj.GetComponent<RomItem>();
            if (bet != null)
            {
                bet.Init(roomDatas[i]);
            }
        }
    }

}