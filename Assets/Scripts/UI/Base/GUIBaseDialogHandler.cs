using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIBaseDialogHandler : MonoBehaviour
{
    [SerializeField] private GameObject black;

    public void Awake()
    {
        OnInit();
    }

    public void Start()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        if (black != null) black.SetActive(false);
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnBeginShow(object parameter)
    {
        if (black != null) black.SetActive(true);
    }

    public virtual void OnEndShow()
    {
    }

    public virtual void OnBeginHide(object parameter)
    {
    }

    public virtual void OnEndHide(bool isDestroy)
    {
        if (black != null) black.SetActive(false);
    }
}