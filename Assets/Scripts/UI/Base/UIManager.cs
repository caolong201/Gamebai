using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public enum DialogName
{
	None = 0,
	UIMessageBox = 1,
	UINotification = 2,
    UIEntertable= 3,
    UICreateTable = 4,
    UIContentSettings = 5,
    UIRulesBox = 6,

}

public class UIManager : SingletonMonoAwake<UIManager>
{
	public Camera UiCamera;
	private List<GUIDialogBase> listDialogs = null;
	private List<GUIDialogBase> activeDialog = new List<GUIDialogBase>();

    [SerializeField] private Transform MatchWidthCanvas;
    [SerializeField] private Transform MatchHeightCanvas;

    public override void OnAwake()
	{
		base.OnAwake();
		listDialogs = new List<GUIDialogBase>(GetComponentsInChildren<GUIDialogBase>());

		StartCoroutine(delayLoad());
	}

	IEnumerator delayLoad()
    {
		yield return new WaitForSeconds(0.1f);
		SortDialog();
		PreLoad();
	}

	private float cameraAspect = -1f;
	public float CameraAspect
	{
		get
		{
			if (cameraAspect == -1)
			{
				cameraAspect = UIManager.Instance.UiCamera.aspect;
				if (cameraAspect > 1) cameraAspect = 1 / cameraAspect;
			}
			return cameraAspect;
		}
	}
	public float GetScaleUI(bool isMathWidth = true)
	{
        if(isMathWidth)
        {
            return MatchWidthCanvas.lossyScale.x;
        }
        else
        {
            return MatchHeightCanvas.lossyScale.x;
        }
		
	}

	
	public bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public bool IsActiveDialog(DialogName dlgName)
	{
		GUIDialogBase foundDlg = listDialogs.Find (dlg => dlg.dialogName == dlgName);
		int index = activeDialog.FindIndex (obj => obj == foundDlg);
		if(index > -1)
			return true;
		return false;
	}

	public bool IsAnyDialogShowed()
	{
		if (activeDialog == null || activeDialog.Count == 0) return false;
		return true;
	}

	public GUIDialogBase ShowDialog (DialogName dlgName, object param = null)
	{
		GUIDialogBase foundDlg = listDialogs.Find (dlg => dlg.dialogName == dlgName);
		if (foundDlg == null) {
			Debug.LogError ("Ko tim thay dialog:" + dlgName);
			return null;
		}
		Debug.Log ("Start show dialog:" + dlgName);
		
		if (!foundDlg.TryShow (param)) {
			Debug.LogError ("Ko the show dialog:" + dlgName);
		} else {
			int index = activeDialog.FindIndex (obj => obj == foundDlg);
			if (index > - 1)
				activeDialog.RemoveAt (index);
			activeDialog.Add (foundDlg);// add vao active
		}

        return foundDlg;
	}

	public void HideDialog (DialogName dlgName, object param = null)
	{
		GUIDialogBase foundDlg = listDialogs.Find (dlg => dlg.dialogName == dlgName);
		if (foundDlg == null) {
			Debug.LogError ("Ko tim thay dialog:" + dlgName);
			return;
		}
		//Debug.LogError(dlgName);
		foundDlg.Hide (param);
	
		int index = activeDialog.FindIndex (obj => obj == foundDlg);
		if (index > - 1)
			activeDialog.RemoveAt (index);

    }

	public GUIDialogBase GetDialog (DialogName dlgName)
	{
		return listDialogs.Find (dlg => dlg.dialogName == dlgName);
	}

	public void HideDialogAfterTime (DialogName dlgName, float delayTime)
	{
		StartCoroutine (CoroutineHideDialog (dlgName, delayTime));
	}

	private IEnumerator CoroutineHideDialog (DialogName dlgName, float delayTime)
	{
		yield return new WaitForSeconds (delayTime);
		HideDialog (dlgName);
	}

	public void HideAllDialog ()
	{
		if (activeDialog != null)
			activeDialog.Clear ();
		if (listDialogs == null)
			return;
	
		foreach (var dlg in listDialogs) {
			if (dlg != null)
				dlg.Hide (null);
		}
	}

	public void SortDialog ()
	{
		
		listDialogs.Sort ((x,y) => x.layer.CompareTo (y.layer));
	
		for (int i = 0; i < listDialogs.Count(); i++) {
			listDialogs [i].transform.SetSiblingIndex (i);
		}
	}

	public void PreLoad()
	{
		for (int i = 0; i < listDialogs.Count(); i++)
		{
			listDialogs[i].Init();
		}
    }

}
