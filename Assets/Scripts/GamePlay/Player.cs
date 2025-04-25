using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public PlayerPosition seatInfo;
    public PlayerInforUI inforUI;
    private List<Card> hand = new List<Card>();


    private void Awake()
    {
        inforUI = GetComponentInChildren<PlayerInforUI>(true);
    }

    public void Init(PlayerPosition info)
    {
        this.seatInfo = info;
        inforUI.Init(info.nickname);
        hand.Clear();

        if (info.position == 0)
        {
            inforUI.transform.localPosition = new Vector2(200, 16);
        }
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
    
    public void SetHand(List<Card> sortHand)
    {
        hand = sortHand;
    }

    // Kiểm tra xem có thể tạo phỏm với lá bài này không
    public bool CanFormPhomWith(Card card)
    {
        // Logic kiểm tra xem lá bài có thể tạo phỏm với các lá bài hiện có
        List<Card> tempHand = new List<Card>(hand) { card };
        List<List<Card>> phoms = FindPhoms(tempHand);
        return phoms.Count > 0;
    }

    // Tìm các phỏm trong tay bài
    public List<List<Card>> FindPhoms(List<Card> cards = null)
    {
        if (cards == null) cards = hand;
        List<List<Card>> phoms = new List<List<Card>>();

        // Tìm bộ ba hoặc bộ bốn cùng giá trị
        var groups = cards.GroupBy(c => c.value).Where(g => g.Count() >= 3);
        foreach (var group in groups)
        {
            phoms.Add(group.ToList());
        }

        // Tìm sảnh cùng chất (cần ít nhất 3 lá liên tiếp)
        var suitGroups = cards.GroupBy(c => c.suit);
        foreach (var suitGroup in suitGroups)
        {
            List<Card> sorted = suitGroup.OrderBy(c => c.value).ToList();
            List<Card> run = new List<Card>();

            for (int i = 0; i < sorted.Count; i++)
            {
                if (run.Count == 0 || sorted[i].value == run.Last().value + 1)
                {
                    run.Add(sorted[i]);
                    if (run.Count >= 3) phoms.Add(new List<Card>(run));
                }
                else
                {
                    run.Clear();
                    run.Add(sorted[i]);
                }
            }
        }

        return phoms;
    }

    // Đánh ra một lá bài
    public void DiscardCard(Card card)
    {
        RemoveCardFromHand(card);
        // Thêm logic để đưa lá bài vào discard pile
    }

    // Đánh ra một phỏm
    public void DropPhom(List<Card> phom)
    {
        foreach (var card in phom)
        {
            RemoveCardFromHand(card);
        }
        // Thêm logic để hiển thị phỏm trên bàn
    }

    // Kiểm tra xem lá bài có thuộc phỏm không
    public bool IsCardInPhom(Card card)
    {
        List<List<Card>> phoms = FindPhoms();
        foreach (var phom in phoms)
        {
            if (phom.Contains(card)) return true;
        }
        return false;
    }
    
    public int CalculateHandScore()
    {
        int score = 0;
        foreach (var card in GetHand())
        {
            score += card.value; // Tính tổng giá trị bài rác
        }
        return score;
    }
    public bool HasCompletedPhom()
    {
        // Kiểm tra xem có tập hợp Phỏm nào hợp lệ không
        return FindPhoms().Count > 0;
    }

    public bool IsMom()
    {
        // Một người chơi bị "Móm" nếu không có bất kỳ Phỏm nào
        return FindPhoms().Count == 0;
    }
}