using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using System.Collections;

public class UIRulesBox : GUIBaseDialogHandler
{
    [SerializeField] Image load;
    public float collapsedHeight = 200f;
    public float expandedHeight = 500f;
    public float animationDuration = 0.3f;

    private Coroutine currentAnim;
    public RectTransform scrollView;
    
    private Action OnCloseClick;
    [SerializeField] private GameObject[] uiElementsToShowAfterLoad;

    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);

        UIRulesBoxData data = (UIRulesBoxData)parameter;
 
        OnCloseClick = data.onCloseClick;

        SetUIElementsActive(false);
        StartCoroutine(RotateLoadThenShowUI());
    }
    private void SetUIElementsActive(bool active)
    {
        foreach (var go in uiElementsToShowAfterLoad)
        {
            if (go != null)
                go.SetActive(active);
        }
    }

    private IEnumerator RotateLoadThenShowUI()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            load.transform.Rotate(Vector3.forward, -360 * Time.deltaTime / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        load.transform.rotation = Quaternion.identity;       
        load.gameObject.SetActive(false);
        SetUIElementsActive(true);
    }
    public void OnbtnCloseClicked()
    {
        OnCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UIRulesBox);
    }

    public class UIRulesBoxData
    {
        public Action onCloseClick;

        public UIRulesBoxData(Action _onOkClick = null)
        {
            onCloseClick = _onOkClick;

        }
        public UIRulesBoxData(Action _onOkClick, Action _onCance)
        {
            onCloseClick = _onOkClick;

        }
    }
}
