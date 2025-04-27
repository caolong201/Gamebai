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

        var (phoms, remainingCards) = FindAllPossiblePhoms(tempHand);
        foreach (var phom in phoms)
        {
            foreach (var c in phom)
            {
                if (c.value == discardedCard.value && c.suit == discardedCard.suit)
                    return true;
            }
        }

        return false;
    }
    
    public static List<List<CardValue>> FindPhoms(List<CardValue> cards)
    {
        List<List<CardValue>> phoms = new List<List<CardValue>>();
    
        if (cards == null || cards.Count < 3)
            return phoms;

        List<CardValue> remainingCards = new List<CardValue>(cards);
    
        // Find rank phoms (3-4 cards of same value)
        var rankGroups = cards.GroupBy(c => c.value)
            .Where(g => g.Count() >= 3);
        foreach (var group in rankGroups)
        {
            var phom = group.ToList();
            phoms.Add(phom);
            remainingCards = remainingCards.Except(phom).ToList();
        }
    
        // Find sequence phoms (3+ consecutive cards of same suit)
        var suitedGroups = remainingCards
            .GroupBy(c => c.type)
            .Where(g => g.Count() >= 3);
    
        foreach (var suitGroup in suitedGroups)
        {
            var sortedCards = suitGroup.OrderBy(c => c.value).ToList();
        
            // Find all possible sequences of length 3 or more
            for (int sequenceLength = sortedCards.Count; sequenceLength >= 3; sequenceLength--)
            {
                for (int startIndex = 0; startIndex <= sortedCards.Count - sequenceLength; startIndex++)
                {
                    if (IsConsecutive(sortedCards, startIndex, sequenceLength))
                    {
                        var phom = sortedCards.GetRange(startIndex, sequenceLength);
                        phoms.Add(phom);
                    
                        // Remove these cards from further consideration
                        for (int i = startIndex; i < startIndex + sequenceLength; i++)
                        {
                            remainingCards.Remove(sortedCards[i]);
                        }
                        break; // Move to next suit group after finding a sequence
                    }
                }
            }
        }

        return phoms;
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
        var suitedGroups = remainingCards.GroupBy(c => c.suit)
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
    
    private static bool IsConsecutive(List<CardValue> cards, int startIndex, int length)
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
        return phom.GroupBy(c => c.suit).Count() == 1 && // Cùng chất
               IsConsecutive(phom.OrderBy(c => c.value).ToList(), 0, phom.Count);
    }

    public static bool IsSameCard(Card card1, Card card2)
    {
        if (card1.suit == card2.suit && card1.value == card2.value) return true;

        return false;
    }
}