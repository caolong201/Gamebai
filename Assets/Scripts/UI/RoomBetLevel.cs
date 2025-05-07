using System;
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
    [SerializeField] RectTransform  footer;
    void Start()
    {     
        footer.anchoredPosition = new Vector2(0, -150);
        footer.DOAnchorPosY(119, .8f).SetEase(Ease.OutCirc);

        int basePrice = 100;
        for (int i = 0; i < 15; i++) 
        {
            //Instantiate(btnPrefab, content);
            GameObject btnObj = Instantiate(btnPrefab, content);

            RomItem bet = btnObj.GetComponent<RomItem>();
            if (bet != null)
            {
                bet.SetPrice(basePrice + i);
                bet.SetDescription(i + 1);
            }
        }
    }
}
