using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public enum ESceneName
{
    Login = 0,
    Room = 1,
    GamePlay = 2,
}

public class SceneFader : SingletonMonoAwake<SceneFader>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float time;
    private bool isLoading = false;

    public ESceneName CurrentScene = ESceneName.Login;
    public void LoadScene(ESceneName sceneName, System.Action callback = null)
    {
        CurrentScene = sceneName;
        isLoading = true;
        UIManager.Instance.HideAllDialog();
        FadeIn(() =>
        {
            StartCoroutine(SceneLoadManager.LoadLevelAsync(sceneName.ToString(), () =>
            {
                Resources.UnloadUnusedAssets();
                FadeOut(() =>
                {
                    callback?.Invoke();
                    isLoading = false;
                });
            }));
        });
    }

    public void LoadSceneNoFadeIn(string sceneName, System.Action callback = null)
    {
        isLoading = true;
        UIManager.Instance.HideAllDialog();

        StartCoroutine(SceneLoadManager.LoadLevelAsync(sceneName, () =>
        {
            Resources.UnloadUnusedAssets();
            FadeOut(() =>
            {
                callback?.Invoke();
                isLoading = false;
            });
        }));
    }

    public void FadeIn(System.Action callback = null)
    {
        canvasGroup.DOKill();

        SetAlpha(0);
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.DOFade(1f, time).OnComplete(() => { callback?.Invoke(); });
    }

    public void FadeOut(System.Action callback = null)
    {
        canvasGroup.DOKill();

        SetAlpha(1);
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.DOFade(0, time).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
            callback?.Invoke();
        });
    }

    public void FadeInOut(System.Action callback = null)
    {
        FadeIn(() => { FadeOut(() => { callback?.Invoke(); }); });
    }

    private void SetAlpha(float val)
    {
        canvasGroup.alpha = val;
    }

    private int cacheTargetVal = 0;


    public bool IsLoadingScene
    {
        get { return isLoading; }
    }
}