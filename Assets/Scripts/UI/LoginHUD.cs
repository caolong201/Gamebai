using System;
using BestHTTP.JSON.LitJson;
using Suni.Enum;
using Suni.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LoginHUD : MonoBehaviour
{
    [SerializeField] GameObject btnLoginGuess, btnPlay, btnLogin, btnDK;
    [SerializeField] LoginPopup loginPopup;

    private int randomID;

    private void Start()
    {
        Application.targetFrameRate = 60;

#if UNITY_EDITOR
        randomID = 0;
#else
        randomID = UnityEngine.Random.Range(0, 9999);
#endif

        btnLoginGuess.SetActive(true);
        btnPlay.SetActive(false);
        NetworkManager.Instance.JoinPhomGame.OnDataUpdated += JoinPhomGame;
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.JoinPhomGame.OnDataUpdated -= JoinPhomGame;
    }

    public void OnbtnLoginGuessClicked()
    {
        string json = JsonMapper.ToJson(new LoginModel((int)ENetworkHeader.LoginGuest, new LoginData()
        {
            username = SystemInfo.deviceUniqueIdentifier + randomID,
            deviceId = SystemInfo.deviceUniqueIdentifier + randomID,
            gameType = (int)EGameType.PHOM
        }));
        NetworkManager.Instance.SendJsonData(json);

    }

    public void OnbtnEnterGameClick()
    {
        string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, 100));
        NetworkManager.Instance.SendJsonData(json);
    }

    public void JoinPhomGame(bool success)
    {
        btnPlay.SetActive(true);

        btnLogin.SetActive(false);
        btnLoginGuess.SetActive(false);
        btnDK.SetActive(false);
        loginPopup.HidePopup();
    }

    public void OnbtnLoginClicked()
    {
        loginPopup.ShowPopup();
    }
}