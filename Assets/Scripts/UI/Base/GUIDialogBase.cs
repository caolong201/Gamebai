using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Utils;
using DG.Tweening;
using MEC;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GUIPanelHideAction
{
    Disable = 0,
    Destroy,
}

public enum EAnimationMove
{
    None,
    InLeft,
    OutLeft,
    InTop,
    OutTop,
    InBot,
    OutBot,
    InRight,
    OutRight,
}

[Serializable]
public class GUIAnimationMove
{
    public EAnimationMove move = EAnimationMove.None;
    public Ease ease = Ease.Linear;
    public float time = .3f;
}
[Serializable]
public class GUIAnimationFade
{
    public bool isFade = false;
    public Ease ease = Ease.Linear;
    public float time = .3f;
}
[Serializable]
public class GUIAnimationScale
{
    public bool isScale = false;
    public float from = 0;
    public float to = 1;
    public float time = .3f;
    public Ease ease = Ease.Linear;
}

public class GUIDialogBase : MonoBehaviour
{
    public enum GUIPanelStatus
    {
        Invalid,
        Ok,
        Showing,
        Showed,
        Hiding,
        Hidden
    }

    public const string GUI_PATH_PREFAB = "Dialogs/";

    // Hide action
    public DialogName dialogName = DialogName.None;
    public string dialogPrefab = "";
    public string locationName = "UIContent";
    public int layer = 0;
    public GUIPanelHideAction hideAction;
    public float destroyTimeout = 120;


    public float showDelay;


    public bool useBlackBackground = true;

    // Active state
    private Dictionary<GameObject, bool> activeSave;
    [HideInInspector]
    public GUIPanelStatus
            status = GUIPanelStatus.Invalid;
    private bool isHasAlpha = false;
    public bool isSetupLocation = false;
    public Vector2 vectorSetupLocation = new Vector2(0, 0);

    [SerializeField] private bool isFullScreen;

    [HideInInspector]
    public GameObject guiControlDlg;
    [HideInInspector]
    public RectTransform guiControlLocation;

    [HideInInspector]
    GUIBaseDialogHandler uiBaseDialogHandler;

    [Header("GUI Animation")]
    [SerializeField] private bool isMathHeight;
    [Header("=============SHOW=============")]
    [SerializeField] private GUIAnimationMove moveShow;
    [SerializeField] private GUIAnimationScale scaleShow;
    [SerializeField] private GUIAnimationFade fadeShow;
    [Header("=============HIDE=============")]
    [SerializeField] private GUIAnimationMove moveHide;
    [SerializeField] private GUIAnimationScale scaleHide;
    [SerializeField] private GUIAnimationFade fadeHide;

    private CanvasGroup guiControlLocationCanvasGroup;
    public GUIBaseDialogHandler GetDialogHandler()
    {
        return uiBaseDialogHandler;
    }

    public virtual GameObject OnInit()
    {
        try
        {
            //Debug.Log("Load dialog " + dialogPrefab);
            GameObject obj = GameObjectUtils.LoadGameObject(gameObject.transform, GUI_PATH_PREFAB + dialogPrefab);
            obj.name = dialogPrefab;
            if (obj.transform.Find(locationName) != null)
            {
                guiControlLocation = obj.transform.Find(locationName).GetComponent<RectTransform>();
                guiControlLocationCanvasGroup = guiControlLocation.GetComponent<CanvasGroup>();
                if(guiControlLocationCanvasGroup == null)
                {
                    guiControlLocationCanvasGroup = guiControlLocation.gameObject.AddComponent<CanvasGroup>() as CanvasGroup;
                }
                if (guiControlLocation == null) Debug.LogError("Could not found RectTransform of UIContent");
                else guiControlLocation.gameObject.SetActive(false);
            }

            uiBaseDialogHandler = gameObject.GetComponentInChildren<GUIBaseDialogHandler>();
        
            if(isFullScreen)
            {
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.anchorMax = Vector2.one;
                rect.anchorMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }

            return obj;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error:" + ex.Message);
        }
        return null;
    }

    /// <summary>
    /// Inits this instance.
    /// </summary>
    public bool Init()
    {
        if (status >= GUIPanelStatus.Ok)
            return true;
        status = GUIPanelStatus.Invalid;

        try
        {
            guiControlDlg = OnInit();
            if (guiControlDlg != null)
            {
                status = GUIPanelStatus.Ok;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Init GUI panel - Init " + this.GetType().Name + "Exception - " + ex);
        }

        return false;
    }


    /// <summary>
    /// Tries the show.
    /// </summary>
    public bool TryShow(object parameter)
    {
        if (status == GUIPanelStatus.Invalid)
        {
            Init();
        }
        
        if (status == GUIPanelStatus.Hiding)
        {
            OnEndHide(hideAction == GUIPanelHideAction.Destroy);
            status = GUIPanelStatus.Hidden;
        }

        if (status == GUIPanelStatus.Hidden || status == GUIPanelStatus.Ok)
        {
            status = GUIPanelStatus.Showing;

            RestoreActiveState(true);
            if (isHasAlpha)
            {
                isHasAlpha = false;
            }
            Timing.RunCoroutine(DoShow(parameter));
            return true;
        }

        if (status == GUIPanelStatus.Showed || status == GUIPanelStatus.Showing)
        {
            //Debug.Log("here");
            DoShowNoAnim(parameter);
            return true;
        }
        return false;
    }

    private IEnumerator<float> DoShow(object parameter)
    {
        yield return Timing.WaitForSeconds(showDelay);
        if (isSetupLocation && guiControlLocation != null)
        {
            Vector3 pos = guiControlLocation.transform.localPosition;
            pos.x = vectorSetupLocation.x;
            pos.y = vectorSetupLocation.y;
            pos.z = 0;
            guiControlLocation.transform.localPosition = pos;
        }
        float wait = OnBeginShow(parameter);

        if (wait > 0)
            yield return Timing.WaitForSeconds(wait);
        else
            yield return Timing.WaitForOneFrame;
        if (status == GUIPanelStatus.Showing)
        {
            OnEndShow();
            status = GUIPanelStatus.Showed;
        }
    }

    private void DoShowNoAnim(object parameter)
    {
        if (uiBaseDialogHandler != null)
        {
            uiBaseDialogHandler.OnBeginShow(parameter);
        }

        OnEndShow();
        status = GUIPanelStatus.Showed;
    }

    /// <summary>
    /// Hides the specified after time out.
    /// </summary>
    public void Hide(object parameter)
    {
        if (gameObject == null)
            return;

        // End showing
        if (status == GUIPanelStatus.Showing)
        {
            OnEndShow();
            status = GUIPanelStatus.Showed;
        }

        if (status == GUIPanelStatus.Showed)
        {
            status = GUIPanelStatus.Hiding;

            SaveActiveState();

            StartCoroutine(DoHide(parameter));
        }
    }
    public void InitialDefaulState()
    {
        status = GUIPanelStatus.Hiding;
        SaveActiveState();
        guiControlDlg.SetActive(false);
    }

    /// <summary>
    /// Does the hide.
    /// </summary>
    private IEnumerator<float> DoHide(object parameter)
    {
        float wait = OnBeginHide(parameter);
        Timing.RunCoroutine(GameObjectUtils._DoActionDelayTime(wait, ()=> {
            if (status == GUIPanelStatus.Hiding)
            {
                OnEndHide(hideAction == GUIPanelHideAction.Destroy);

                switch (hideAction)
                {
                    case GUIPanelHideAction.Disable:
                        guiControlDlg.SetActive(false);
                        status = GUIPanelStatus.Hidden;
                        break;

                    case GUIPanelHideAction.Destroy:
                        guiControlDlg.SetActive(false);
                        status = GUIPanelStatus.Hidden;
                        Timing.RunCoroutine(WaitForDestroy());
                        break;
                }
            }
        }));

        yield return Timing.WaitForOneFrame;
    }

    IEnumerator<float> WaitForDestroy()
    {
        yield return Timing.WaitForSeconds(destroyTimeout);
        Destroy(guiControlDlg);
        status = GUIPanelStatus.Invalid;
    }

    protected float DoTweenShow()
    {
        float time = 0;
        if (moveShow.move != EAnimationMove.None)
        {
            switch (moveShow.move)
            {
                case EAnimationMove.InLeft:
                    guiControlLocation.anchoredPosition = new Vector2(-200 - Screen.width/UIManager.Instance.GetScaleUI(!isMathHeight), 0);
                    break;
                case EAnimationMove.InRight:
                    guiControlLocation.anchoredPosition = new Vector2(200 + Screen.width / UIManager.Instance.GetScaleUI(!isMathHeight), 0);
                    break;
                case EAnimationMove.InTop:
                    guiControlLocation.anchoredPosition = new Vector2(0, 300 + Screen.height / UIManager.Instance.GetScaleUI(!isMathHeight));
                    break;
                case EAnimationMove.InBot:
                    guiControlLocation.anchoredPosition = new Vector2(0, -300- Screen.height / UIManager.Instance.GetScaleUI(!isMathHeight));
                    break;
            }
            guiControlLocation.gameObject.SetActive(true);
            guiControlLocation.DOAnchorPos(new Vector3(0, 0, 0),moveShow.time).SetEase(moveShow.ease);
            time = moveShow.time;
        }

        if (scaleShow.isScale)
        {
            guiControlLocation.localScale = new Vector3(scaleShow.from, scaleShow.from, 1);
            guiControlLocation.gameObject.SetActive(true);
            guiControlLocation.DOScale(new Vector3(scaleShow.to, scaleShow.to, 1),scaleShow.time).SetEase(scaleShow.ease);
            if(time < scaleShow.time) time = scaleShow.time;
        }

        if(fadeShow.isFade)
        {
            guiControlLocationCanvasGroup.alpha = 0;
            guiControlLocation.gameObject.SetActive(true);
            guiControlLocationCanvasGroup.DOFade(1, fadeShow.time).SetEase(fadeShow.ease);
            if (time < fadeShow.time) time = fadeShow.time;
        }

        if (moveShow.move == EAnimationMove.None && !scaleShow.isScale && !fadeShow.isFade)
        {
            guiControlLocation.gameObject.SetActive(true);
        }
        
        return time;
    }

    protected float DoTweenHide()
    {
        float time = 0;
        if (moveHide.move != EAnimationMove.None)
        {
            switch (moveHide.move)
            {
                case EAnimationMove.OutLeft:
                    guiControlLocation.DOAnchorPos(new Vector3(-200 - Screen.width / UIManager.Instance.GetScaleUI(!isMathHeight), 0, 0), moveHide.time).SetEase(moveHide.ease);
                    break;
                case EAnimationMove.OutRight:
                    guiControlLocation.DOAnchorPos(new Vector3(200 + Screen.width / UIManager.Instance.GetScaleUI(!isMathHeight), 0, 0), moveHide.time).SetEase(moveHide.ease);
                    break;
                case EAnimationMove.OutTop:
                    guiControlLocation.DOAnchorPos(new Vector3(0, 300 + Screen.height / UIManager.Instance.GetScaleUI(!isMathHeight), 0), moveHide.time).SetEase(moveHide.ease);
                    break;
                case EAnimationMove.OutBot:
                    guiControlLocation.DOAnchorPos(new Vector3(0, -300 - Screen.height / UIManager.Instance.GetScaleUI(!isMathHeight), 0), moveHide.time).SetEase(moveHide.ease);
                    break;
            }
            time = moveHide.time;
        }

        if (scaleHide.isScale)
        {
            guiControlLocation.DOScale(new Vector3(scaleHide.to, scaleHide.to, 1), scaleHide.time).SetEase(scaleHide.ease);

            if (time < scaleHide.time) time = scaleHide.time;
        }
        else if (scaleShow.isScale)
        {
            guiControlLocation.localScale = new Vector3(scaleShow.from, scaleShow.from, 1);
        }

        if (fadeHide.isFade)
        {
            guiControlLocationCanvasGroup.DOFade(0, fadeHide.time).SetEase(fadeHide.ease);
            if (time < fadeHide.time) time = fadeHide.time;
        }
        else if (fadeShow.isFade)
        {
            guiControlLocationCanvasGroup.alpha = 0;
        }
        return time;
    }

    /// <summary>
    /// Called when [begin show].
    /// </summary>
    protected virtual float OnBeginShow(object parameter)
    {
        if (uiBaseDialogHandler != null)
        {
            uiBaseDialogHandler.OnBeginShow(parameter);
        }
        //return DoTween(showTweenName);
        return DoTweenShow();
    }

    /// <summary>
    /// Called when [end show].
    /// </summary>
    protected virtual void OnEndShow()
    {
        if (uiBaseDialogHandler != null)
        {
            uiBaseDialogHandler.OnEndShow();
        }
    }

    /// <summary>
    /// Called when [begin hide].
    /// </summary>
    protected virtual float OnBeginHide(object parameter)
    {
        if (uiBaseDialogHandler != null)
        {
            uiBaseDialogHandler.OnBeginHide(parameter);
        }
        return DoTweenHide();
    }

    /// <summary>
    /// Called when [end hide].
    /// </summary>
    protected virtual void OnEndHide(bool isDestroy)
    {
        if (uiBaseDialogHandler != null)
        {
            uiBaseDialogHandler.OnEndHide(isDestroy);
        }
    }

    /// <summary>
    /// Saves the state of the active.
    /// </summary>
    protected void SaveActiveState()
    {
        if (guiControlDlg == null)
            return;

        if (activeSave == null)
            activeSave = new Dictionary<GameObject, bool>();
        else
            activeSave.Clear();

        activeSave[guiControlDlg] = gameObject.activeSelf;

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            activeSave[child.gameObject] = child.gameObject.activeSelf;
        }
    }

    /// <summary>
    /// Restores the state of the active.
    /// </summary>
    protected void RestoreActiveState(bool defaultState)
    {
        if (gameObject == null || activeSave == null)
            return;

        bool isActive = false;
        if (activeSave.TryGetValue(gameObject, out isActive))
            gameObject.SetActive(isActive);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (activeSave.TryGetValue(child.gameObject, out isActive))
                child.gameObject.SetActive(isActive);
            else
                child.gameObject.SetActive(defaultState);
        }
    }

    public void ApplyDepthPosition(GameObject guiObject)
    {
        if (guiObject == null)
            return;
        if (layer >= 15)
        {
            layer = 15;
        }
        Vector3 pos = guiObject.transform.localPosition;
        if (layer >= 0)
        {
            pos.z = -20 - layer * 10;
        }
        else
        {
            pos.z = -20 - 150;
        }
        guiObject.transform.localPosition = pos;
        transform.SetSiblingIndex(layer);
    }
}
