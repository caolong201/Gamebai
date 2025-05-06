using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimMoveUpDown : MonoBehaviour
{
    [SerializeField] float _duration = 0.5f;
    [SerializeField] float dictance = 20f;
    private void OnEnable()
    {
        transform.DOMoveY(transform.position.y - dictance, _duration).SetLoops(-1, LoopType.Yoyo);
    }
    
    private void OnDisable()
    {
        transform.DOKill();
    }
}
