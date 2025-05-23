using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public PlayerPosition seatInfo;
    public PlayerInforUI inforUI;
    private List<Card> hand = new List<Card>();
    private List<Card> phoms = new List<Card>();
    private float cardSpacing = 125;

    private string[] suits = { "♥", "♦", "♣", "♠" };

    [HideInInspector] public GuiBaiInfo SendCards;

    private void Awake()
    {
        inforUI = GetComponentInChildren<PlayerInforUI>(true);
    }

    public void Init(PlayerPosition info)
    {
        this.seatInfo = info;
        inforUI.Init(info);
        hand.Clear();
        if (info.position == 0)
        {
            inforUI.transform.position = new Vector2(Screen.width / 2, inforUI.transform.position.y);
        }

        phoms.Clear();
        SendCards = null;
    }

    // Thêm lá bài vào tay
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        card.transform.SetParent(transform);
    }

    // Xóa lá bài khỏi tay
    public void RemoveCardFromHand(Card card)
    {
        hand.Remove(card);
    }

    // Lấy danh sách lá bài trong tay
    public List<Card> GetHand()
    {
        return hand;
    }

    public Card FindCard(CardValue cardValue)
    {
        return GetHand().Find(x => x.value == cardValue.value && x.suit == suits[cardValue.type - 1]);
    }

    public void SetHand(List<Card> sortHand)
    {
        hand = sortHand;
    }

    public void AddPhoms(Card card)
    {
        this.phoms.Add(card);
    }

    public List<Card> GetPhoms()
    {
        return phoms;
    }

    public void SortPhoms()
    {
        var sort = PhomChecker.FindPhoms(phoms);
        float startY = 0f;
        RectTransform rt;
        foreach (var phom in sort)
        {
            float startX = -(phom.Count + (phom.Count - 1) * cardSpacing) / 2f;

            // Tạo từng lá bài trong phỏm
            for (int i = 0; i < phom.Count; i++)
            {
                Card card = phom[i];
                rt = card.GetComponent<RectTransform>();

                // Tính vị trí thủ công
                Vector2 target = new Vector2(
                    startX + i * (cardSpacing / 1.5f),
                    startY
                );
                rt.DOAnchorPos(target, 0.3f);
            }

            // Xuống dòng cho phỏm tiếp theo
            startY -= 70;
        }
    }
}