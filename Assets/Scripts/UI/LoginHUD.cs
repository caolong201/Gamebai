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
    [SerializeField] LoginPopup loginPopup;
    [SerializeField] RegisterPopup registerPopup;
    
    private int randomID;

    private void Start()
    {

#if UNITY_EDITOR
        randomID = 0;
#else
        randomID = UnityEngine.Random.Range(0, 9999);
#endif

        NetworkManager.Instance.JoinPhomGame.OnDataUpdated += JoinPhomGame;
        NetworkManager.Instance.Register.OnDataUpdated += RegisterSuccess;
    }
    
    private void OnDestroy()
    {
        NetworkManager.Instance.JoinPhomGame.OnDataUpdated -= JoinPhomGame;
        NetworkManager.Instance.Register.OnDataUpdated -= RegisterSuccess;
    }

    public void OnbtnLoginGuessClicked()
    {
        string json = JsonMapper.ToJson(new LoginModel((int)ENetworkHeader.LoginGuest, new LoginData()
        {
            username = SystemInfo.deviceUniqueIdentifier + randomID,
            deviceId = SystemInfo.deviceUniqueIdentifier + randomID,
            gameType = (int)EGameType.PHOM,
            version = "0.1"
        }));
        NetworkManager.Instance.SendJsonData(json);

    }

    public void JoinPhomGame(bool success)
    {
        loginPopup.HidePopup();
        SceneFader.Instance.LoadScene(ESceneName.Room);
    }

    public void OnbtnLoginClicked()
    {
        loginPopup.ShowPopup();
    }
    
    public void OnbtnRegisterClicked()
    {
        registerPopup.ShowPopup();
    }
    
    private void RegisterSuccess(bool obj)
    {
        registerPopup.HidePopup();
        UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData("Đăng ký thành công!","OK", () =>
        {
            loginPopup.ShowPopup();
        }));
       
    }

}