using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardEffect : MonoBehaviour
{
    private Image effectImage;

    private void OnEnable()
    {
        if (effectImage == null) effectImage = GetComponent<Image>();
        if (effectImage != null)
        {
            effectImage.DOKill();
            effectImage.DOFade(0f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}