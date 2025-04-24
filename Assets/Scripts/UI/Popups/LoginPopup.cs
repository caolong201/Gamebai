
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using UnityEngine;

public class LoginPopup : MonoBehaviour
{
    [SerializeField] private GameObject black, root;

    public TMP_InputField ipUsername;
    public TMP_InputField ipPassword;
    public void ShowPopup()
    {
        gameObject.SetActive(true);
        black.SetActive(transform);
        root.transform.localScale = Vector3.one * 0.5f;
        root.transform.DOScale(Vector3.one, 0.3f);

        ipUsername.text = "";
        ipPassword.text = "";
    }

    public void OnbtnLoginClicked()
    {
        if(string.IsNullOrEmpty(ipUsername.text) || string.IsNullOrEmpty(ipPassword.text))
            //show popup error later
            return;
        
        Debug.Log("ipUsername: " + ipUsername.text + ", ipPassword: " + ipPassword.text);
        string json = JsonMapper.ToJson(new LoginModel((int)ENetworkHeader.LoginGuest, new LoginData()
        {
            username = SystemInfo.deviceUniqueIdentifier + ipUsername.text,
            deviceId = SystemInfo.deviceUniqueIdentifier + ipUsername.text,
            gameType = (int)EGameType.PHOM
        }));
        NetworkManager.Instance.SendJsonData(json);
    }

    public void HidePopup()
    {
        root.transform.DOScale(Vector3.one * 0.5f, 0.2f).OnComplete(() =>
        {
            black.SetActive(false);
            gameObject.SetActive(false);
        });
    }
}
