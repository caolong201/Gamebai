using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RomItem : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI textperson;


    public void SetPrice(int price)
    {
        if (priceText != null)
        {
            priceText.text = price.ToString();
        }
       
    }
    public void SetDescription(int index)
    {
        if (textperson != null)
        {
            textperson.text = index.ToString();
        }
    }
}
