using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int value; // Giá trị lá bài (1 = A, 11 = J, 12 = Q, 13 = K)
    public string suit; // Chất: "♠", "♥", "♦", "♣"
    public TextMeshProUGUI txtSuitBig, txtSuitSmall, txtValue; // UI để hiển thị giá trị bài

    public GameObject cardFront;
    public CardEffect cardEffect;
    public void SetCard(int cardValue, string cardSuit)
    {
        cardFront.SetActive(false);
        value = cardValue;
        suit = cardSuit;
        
        txtSuitBig.text = $"{suit}";
        txtSuitSmall.text = $"{suit}";
        txtValue.text = $"{GetCardString()}";

        if (suit == "\u2663" || suit == "\u2660")
        {
            txtSuitBig.color = Color.black;
            txtSuitSmall.color = Color.black;
            txtValue.color = Color.black;
        }
        else
        {
            txtSuitBig.color = Color.red;
            txtSuitSmall.color = Color.red;
            txtValue.color = Color.red;
        }
    }

    public void Up()
    {
        cardFront.SetActive(true);
    }
    public void ShowEffect(bool isShow)
    {
        cardEffect.gameObject.SetActive(isShow);
    }

    public void Down()
    {
        cardFront.SetActive(false);
    }

    private string GetCardString()
    {
        switch (value)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return value.ToString();
        }
    }
}