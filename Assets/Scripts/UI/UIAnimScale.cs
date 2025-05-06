using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimScale : MonoBehaviour
{
    [SerializeField] private bool isUp = false;

    private void OnEnable()
    {
        if (isUp) transform.DOScale(new Vector3(1.1f, 1.1f), .4f).SetLoops(-1, LoopType.Yoyo);
        else
        {
            transform.DOScale(new Vector3(0.9f, 0.9f), .4f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}