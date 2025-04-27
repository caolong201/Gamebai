using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIBaseDialogHandler : MonoBehaviour {


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


    }

    public virtual void OnInit()
	{

	}

    public virtual void OnBeginShow(object parameter)
    {
       
    }

	public virtual void OnEndShow()
	{

	}

    public virtual void OnBeginHide(object parameter)
    {

      
    }

    public virtual void OnEndHide(bool isDestroy)
    {

    }
}
