using System;
using System.Collections.Generic;
using System.Linq;

public class PhomChecker
{

    // Hàm chính kiểm tra bài đánh có tạo phỏm không
    public static bool CanFormPhom(List<Card> myCards, Card discardedCard)
    {
        // Tạo danh sách bài tạm thời (bài trên tay + bài đối phương đánh)
        List<Card> tempHand = new List<Card>(myCards) { discardedCard };

        // Kiểm tra phỏm cùng hàng (3+ lá cùng giá trị)
        if (HasSameRankPhom(tempHand, discardedCard.value))
            return true;

        // Kiểm tra phỏm sảnh (3+ lá liên tiếp cùng chất)
        if (HasStraightPhom(tempHand, GetSuitNumber(discardedCard.suit)))
            return true;

        return false;
    }

    // Kiểm tra phỏm cùng hàng
    private static bool HasSameRankPhom(List<Card> cards, int targetRank)
    {
        // Đếm số lá bài cùng giá trị với lá bài đánh ra
        int count = cards.Count(c => c.value == targetRank);
        return count >= 3; // Phỏm hợp lệ khi có 3+ lá cùng giá trị
    }

    // Kiểm tra phỏm sảnh
    private static bool HasStraightPhom(List<Card> cards, int targetSuit)
    {
        // Lọc bài cùng chất với lá bài đánh ra
        var suitedCards = cards.Where(c => GetSuitNumber(c.suit) == targetSuit)
            .OrderBy(c => c.value)
            .ToList();

        // Kiểm tra sảnh 3+ lá liên tiếp
        for (int i = 0; i <= suitedCards.Count - 3; i++)
        {
            if (suitedCards[i].value + 1 == suitedCards[i + 1].value &&
                suitedCards[i].value + 2 == suitedCards[i + 2].value)
            {
                return true;
            }
        }
        return false;
    }
    
     public static List<Card> SortHand(List<Card> hand)
    {
        List<List<Card>> phoms = FindPhoms(hand);
        List<Card> cardsInPhoms = phoms.SelectMany(p => p).ToList();

        // Bài còn lại sau khi đã lấy phỏm ra
        List<Card> remainingCards = hand.Except(cardsInPhoms).ToList();

        // Tìm bài có thể tạo phỏm tiếp
        List<Card> potentialPhoms = FindPotentialPhomCards(remainingCards);

        // Bài rác còn lại
        List<Card> trashCards = remainingCards.Except(potentialPhoms)
                                              .OrderBy(c => c.value)
                                              .ThenBy(c => c.suit)
                                              .ToList();

        // Kết quả cuối cùng
        List<Card> sortedHand = new List<Card>();
        sortedHand.AddRange(cardsInPhoms);
        sortedHand.AddRange(potentialPhoms);
        sortedHand.AddRange(trashCards);

        return sortedHand;
    }

    // Tìm phỏm sẵn có (3 lá cùng số hoặc 3 lá liên tiếp cùng chất)
    public static List<List<Card>> FindPhoms(List<Card> cards)
    {
        List<List<Card>> phoms = new List<List<Card>>();

        // Tìm phỏm 3 lá cùng số
        var sameValueGroups = cards.GroupBy(c => c.value)
                                   .Where(g => g.Count() >= 3);
        foreach (var group in sameValueGroups)
        {
            phoms.Add(group.ToList());
        }

        // Tìm phỏm dây cùng chất
        var typeGroups = cards.GroupBy(c => c.suit);
        foreach (var group in typeGroups)
        {
            var sorted = group.OrderBy(c => c.value).ToList();
            List<Card> currentPhom = new List<Card>();

            for (int i = 0; i < sorted.Count; i++)
            {
                if (currentPhom.Count == 0 || sorted[i].value == currentPhom.Last().value + 1)
                {
                    currentPhom.Add(sorted[i]);
                }
                else
                {
                    if (currentPhom.Count >= 3) phoms.Add(new List<Card>(currentPhom));
                    currentPhom.Clear();
                    currentPhom.Add(sorted[i]);
                }
            }
            if (currentPhom.Count >= 3) phoms.Add(currentPhom);
        }

        return phoms;
    }
    
    public static List<List<CardValue>> FindPhoms(List<CardValue> cards)
    {
        List<List<CardValue>> phoms = new List<List<CardValue>>();

        // Tìm phỏm 3 lá cùng số
        var sameValueGroups = cards.GroupBy(c => c.value)
            .Where(g => g.Count() >= 3);
        foreach (var group in sameValueGroups)
        {
            phoms.Add(group.ToList());
        }

        // Tìm phỏm dây cùng chất
        var typeGroups = cards.GroupBy(c => c.type);
        foreach (var group in typeGroups)
        {
            var sorted = group.OrderBy(c => c.value).ToList();
            List<CardValue> currentPhom = new List<CardValue>();

            for (int i = 0; i < sorted.Count; i++)
            {
                if (currentPhom.Count == 0 || sorted[i].value == currentPhom.Last().value + 1)
                {
                    currentPhom.Add(sorted[i]);
                }
                else
                {
                    if (currentPhom.Count >= 3) phoms.Add(new List<CardValue>(currentPhom));
                    currentPhom.Clear();
                    currentPhom.Add(sorted[i]);
                }
            }
            if (currentPhom.Count >= 3) phoms.Add(currentPhom);
        }

        return phoms;
    }

    // Tìm bài có khả năng tạo phỏm tiếp (2 lá cùng số hoặc 2 lá liên tiếp cùng chất)
    private static List<Card> FindPotentialPhomCards(List<Card> cards)
    {
        List<Card> potential = new List<Card>();

        // 2 lá cùng số
        var sameValueGroups = cards.GroupBy(c => c.value)
                                   .Where(g => g.Count() == 2);
        foreach (var group in sameValueGroups)
        {
            potential.AddRange(group);
        }

        // 2 lá liên tiếp cùng chất
        var typeGroups = cards.GroupBy(c => c.suit);
        foreach (var group in typeGroups)
        {
            var sorted = group.OrderBy(c => c.value).ToList();
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                if (sorted[i + 1].value == sorted[i].value + 1)
                {
                    if (!potential.Contains(sorted[i])) potential.Add(sorted[i]);
                    if (!potential.Contains(sorted[i + 1])) potential.Add(sorted[i + 1]);
                }
            }
        }

        return potential;
    }
    
    public static int GetSuitNumber(string suit)
    {
        switch (suit)
        {
            case "♥": return 1;
            case "♦": return 2;
            case "♣": return 3;
            case "♠": return 4;
            default: return 0;
        }
    }
    
    //////////////////
     public static List<Card> SortCards(List<Card> hand)
    {
        // Bước 1: Tìm tất cả phỏm có thể có trong bài
        var (phoms, remainingCards) = FindAllPossiblePhoms(hand);

        // Bước 2: Sắp xếp phỏm theo độ ưu tiên
        // var sortedPhoms = phoms.OrderByDescending(p => p.Count) // Ưu tiên phỏm dài (4 lá > 3 lá)
        //     .ThenByDescending<List<Card>, object>(p => IsStraightPhom(p).ToList();
        
        var sortedPhoms = phoms
            .OrderByDescending(p => p.Count)
            .ThenByDescending(p => IsStraightPhom(p)) // sắp theo số lượng lá trong phỏm sảnh
            .ToList();

        // Bước 3: Sắp xếp bài rác có thể tạo phỏm
        var (potentialPhomCards, pureRacCards) = FindPotentialPhomCards2(remainingCards);

        // Bước 4: Sắp xếp bài rác theo điểm (ưu tiên tổng điểm nhỏ -> lớn)
        var sortedRac = pureRacCards.OrderBy(c => c.value) // Nhỏ -> lớn
                                   .ToList();

        // Bước 5: Ghép kết quả cuối cùng
        List<Card> finalHand = new List<Card>();
        foreach (var phom in sortedPhoms) finalHand.AddRange(phom);
        finalHand.AddRange(potentialPhomCards);
        finalHand.AddRange(sortedRac);

        return finalHand.Distinct().ToList();
    }

    // Tìm tất cả phỏm có thể trong bài
    private static (List<List<Card>> phoms, List<Card> remainingCards) FindAllPossiblePhoms(List<Card> hand)
    {
        List<List<Card>> phoms = new List<List<Card>>();
        List<Card> remainingCards = new List<Card>(hand);

        // Tìm phỏm cùng hàng (3-4 lá cùng rank)
        var rankGroups = hand.GroupBy(c => c.value)
                            .Where(g => g.Count() >= 3);
        foreach (var group in rankGroups)
        {
            var phom = group.ToList();
            phoms.Add(phom);
            remainingCards = remainingCards.Except(phom).ToList();
        }

        // Tìm phỏm sảnh (3+ lá liên tiếp cùng chất)
        var suitedGroups = hand.GroupBy(c => c.suit)
                              .Where(g => g.Count() >= 3);
        foreach (var suitGroup in suitedGroups)
        {
            var sortedCards = suitGroup.OrderBy(c => c.value).ToList();
            for (int i = 0; i <= sortedCards.Count - 3; i++)
            {
                // Kiểm tra nhóm 3+ lá liên tiếp
                if (IsConsecutive(sortedCards, i, 3))
                {
                    var phom = sortedCards.GetRange(i, 3);
                    phoms.Add(phom);
                    remainingCards = remainingCards.Except(phom).ToList();
                }
            }
        }

        return (phoms, remainingCards);
    }

    // Kiểm tra bài rác có tiềm năng tạo phỏm
    private static (List<Card> potentialPhomCards, List<Card> pureRacCards) FindPotentialPhomCards2(List<Card> cards)
    {
        List<Card> potential = new List<Card>();
        List<Card> pureRac = new List<Card>();

        foreach (var card in cards)
        {
            // Đếm số lá bài cùng rank hoặc có thể tạo sảnh
            int sameRankCount = cards.Count(c => c.value == card.value);
            int sameSuitPotential = GetStraightPotential(cards, card);

            if (sameRankCount >= 2 || sameSuitPotential >= 2)
                potential.Add(card);
            else
                pureRac.Add(card);
        }

        // Sắp xếp bài tiềm năng theo điểm (rank nhỏ -> lớn)
        potential = potential.OrderBy(c => c.value).ToList();
        return (potential, pureRac);
    }

    // Kiểm tra sảnh tiềm năng
    private static int GetStraightPotential(List<Card> cards, Card targetCard)
    {
        var suitedCards = cards.Where(c => c.suit == targetCard.suit)
                              .OrderBy(c => c.value)
                              .ToList();
        int maxPotential = 0;

        for (int i = 0; i < suitedCards.Count; i++)
        {
            if (Math.Abs(suitedCards[i].value - targetCard.value) <= 2)
                maxPotential++;
        }

        return maxPotential;
    }

    // Helper: Kiểm tra dãy liên tiếp
    private static bool IsConsecutive(List<Card> cards, int startIndex, int length)
    {
        for (int i = 1; i < length; i++)
        {
            if (cards[startIndex + i].value != cards[startIndex].value + i)
                return false;
        }
        return true;
    }

    // Helper: Kiểm tra phỏm sảnh
    private static bool IsStraightPhom(List<Card> phom)
    {
        return phom.GroupBy(c => c.suit).Count() == 1 &&  // Cùng chất
               IsConsecutive(phom.OrderBy(c => c.value).ToList(), 0, phom.Count);
    }
}