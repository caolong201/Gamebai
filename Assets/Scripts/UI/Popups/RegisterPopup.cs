
using System;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using UnityEngine;

public class RegisterPopup : MonoBehaviour
{
    [SerializeField] private GameObject black, root;

    public TMP_InputField ipUsername;
    public TMP_InputField ipPassword;
    public TMP_InputField ipConfirmPassword;
    public TMP_InputField ipDisplayName;
    
    public void ShowPopup()
    {
        gameObject.SetActive(true);
        black.SetActive(transform);
        root.transform.localScale = Vector3.one * 0.5f;
        root.transform.DOScale(Vector3.one, 0.3f);

        ipUsername.text = "";
        ipPassword.text = "";
        ipConfirmPassword.text = "";
        ipDisplayName.text = "";
    }

    public void OnbtnRegisterClicked()
    {
        string err = String.Empty;
        
        if(string.IsNullOrEmpty(ipUsername.text))
            err = "Tên đăng nhập bắt buộc";
        
        if(string.IsNullOrEmpty(ipPassword.text))
            err = "Mật khẩu bắt buộc";
        
        if(ipPassword.text != ipConfirmPassword.text)
            err = "Mật khẩu không khớp";
        
        if(string.IsNullOrEmpty(ipDisplayName.text))
            err = "Tên hiển thị bắt buộc";
        
        if(ipUsername.text.Length < 6 || ipUsername.text.Length > 128)
            err = "Tên đăng nhập độ dài từ 6 đến 128 ký tự";
        
        if(ipPassword.text.Length < 6)
            err = "Mật khẩu độ dài từ 6 ký tự trở lên";

        if(ipDisplayName.text.Length < 6 || ipDisplayName.text.Length > 64)
            err = "Tên hiển thị độ dài từ 6 đến 64 ký tự";

        if (!string.IsNullOrEmpty(err))
        {
            UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(err));
            return;
        }

        Debug.Log("ipUsername: " + ipUsername.text + ", ipPassword: " + ipPassword.text);
        string json = JsonMapper.ToJson(new RegisterModel((int)ENetworkHeader.Register, new RegisterModelData()
        {
            username = ipUsername.text.Trim(),
            password = ipPassword.text.Trim(),
            gameType = (int)EGameType.PHOM,
            nickname = ipDisplayName.text.Trim(),
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
