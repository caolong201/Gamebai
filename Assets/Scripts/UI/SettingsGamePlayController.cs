using DG.Tweening;
using Suni.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIRulesBox;

public class SettingsGamePlayController : MonoBehaviour
{
    public GameObject btnSetting;
    public GameObject settingPanel;
    public RectTransform panelParent;

    public void OnSettingButtonClick()
    {
        btnSetting.SetActive(false);
        panelParent.anchoredPosition = new Vector2(-317, -69);
        panelParent.DOAnchorPosX(287, 0.3f).SetEase(Ease.OutCirc);
    }
    public void OnCloseSettingClick()
    {
        btnSetting.SetActive(false);
        panelParent.anchoredPosition = new Vector2(287, -69);
        panelParent.DOAnchorPosX(-317, 0.3f).SetEase(Ease.OutCirc);
        btnSetting.SetActive(true);
    }
    public void OnBackButtonClick()
    {     
        panelParent.anchoredPosition = new Vector2(287, -69);
        panelParent.DOAnchorPosY(191, 0.3f).SetEase(Ease.OutCirc);
        PlayerPrefs.SetString("ReturnAction", "ShowRoomBetLevel");
        SceneFader.Instance.LoadScene(ESceneName.Room, () =>
        {
            Debug.Log("Đã back về RoomBetLevel");
        });
    }

    public void OnbtSetingContent()
    {
        UIManager.Instance.ShowDialog(DialogName.UIContentSettings, new UIContentSettingsData(() =>
                {
                    Debug.Log("Onsetting");
                }));
    }

    public void OnbtRules()
    {
        settingPanel.SetActive(false);
        btnSetting.SetActive(true );
        UIManager.Instance.ShowDialog(DialogName.UIRulesBox, new UIRulesBoxData(() =>
        {
            Debug.Log("UIRulesBox");
        }));
    }
}
   
