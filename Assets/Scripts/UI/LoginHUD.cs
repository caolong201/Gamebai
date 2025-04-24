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
    [SerializeField] GameObject btnLogin, btnPlay;
    [SerializeField] GameObject imgDn;
    public TMP_InputField inputField;
    public TMP_InputField passwordInput;
    private RectTransform textArea;
    //public InputField inputField;
    private int randomID;

    private void Start()
    {
        //textArea = inputField.transform.Find("Text").GetComponent<RectTransform>();
        //inputField.onSelect.AddListener(OnFocus);
        //inputField.onDeselect.AddListener(OnUnfocus);



        Application.targetFrameRate = 60;
        
#if UNITY_EDITOR
        randomID = 0;
#else
        randomID = UnityEngine.Random.Range(0, 9999);
#endif
        
        btnLogin.SetActive(true);
        btnPlay.SetActive(false);
        NetworkManager.Instance.JoinPhomGame.OnDataUpdated += JoinPhomGame;
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.JoinPhomGame.OnDataUpdated -= JoinPhomGame;
    }

    public void OnbtnLoginClick()
    {
        string json = JsonMapper.ToJson(new LoginModel((int)ENetworkHeader.LoginGuest, new LoginData()
        {
            username = SystemInfo.deviceUniqueIdentifier + randomID,
            deviceId = SystemInfo.deviceUniqueIdentifier + randomID,
            gameType = (int)EGameType.PHOM
        }));
        NetworkManager.Instance.SendJsonData(json);

        btnLogin.SetActive(false);
    }

    public void OnbtnEnterGameClick()
    {
        string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, 100));
        NetworkManager.Instance.SendJsonData(json);
    }

    public void JoinPhomGame(bool success)
    {
        btnPlay.SetActive(true);
    }



    //long
    public void OnbtnDangnhap ()
    {
        Debug.Log("c");
        imgDn.SetActive(true);
      
    }
    public void btnClose()
    {
        imgDn.SetActive(false);
    }
    public void PrintInput()
    {
        string userInput = inputField.text;
        string userInputpassword = passwordInput.text;

        Debug.Log("Ten dang nhập: " + userInput);
        Debug.Log ("mat khau:" + userInputpassword);
    }



    //void OnFocus(string text)
    //{
    //    textArea.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack); // scale lên 110%
    //}

    //void OnUnfocus(string text)
    //{
    //    textArea.DOScale(1f, 0.2f).SetEase(Ease.OutBack); // trả lại kích thước gốc
    //}
}