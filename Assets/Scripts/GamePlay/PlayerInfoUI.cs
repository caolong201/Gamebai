using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInforUI : MonoBehaviour
{
    [SerializeField] Image imgTimer;
    private float duration = 60;

    [SerializeField] private TextMeshProUGUI txtNickname, txtCoin;
    [SerializeField] Image imgAvatar;
    private bool isAvatarLoaded = false;

    [SerializeField] private Image imgMom;
    [SerializeField] private List<GameObject> rankUIs;

    public void Init(PlayerPosition info)
    {
        imgTimer.fillAmount = 0;
        txtNickname.text = info.nickname;
        LoadRandomAvatar(info.avatarUrl);
        txtCoin.text = info.coin.FormatCoins();
        StopTimer();
        //reset rank
        imgMom.gameObject.SetActive(false);
        foreach (var rank in rankUIs)
        {
            if (rank != null) rank.SetActive(false);
        }
    }

    public void LoadRandomAvatar(string avatar)
    {
        string avatarName = "avatar_" + (string.IsNullOrEmpty(avatar)? "0" : avatar);
        // Try loading the avatar
        Sprite loadedAvatar = Resources.Load<Sprite>("Avatar/" + avatarName);

        if (loadedAvatar != null)
        {
            imgAvatar.sprite = loadedAvatar;
        }
        else
        {
            Debug.LogError("Failed to load avatar: " + avatarName);
        }
    }

    public void StartTimer()
    {
        imgTimer.enabled = true;
        imgTimer.DOKill();
        imgTimer.fillAmount = 1;
        imgTimer.DOFillAmount(0, duration).SetEase(Ease.Linear);
    }

    public void StopTimer()
    {
        imgTimer.DOKill();
        imgTimer.fillAmount = 1;
        imgTimer.enabled = false;
    }

    public void ShowMom()
    {
        if (imgMom != null)
        {
            imgMom.gameObject.SetActive(true);
            imgMom.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f).SetEase(Ease.OutQuad);
        }
    }

    public void ShowRank(int rank)
    {
        if (imgMom.gameObject.activeSelf) return;
        if (rank >= rankUIs.Count) return;

        rankUIs[rank].SetActive(true);
        rankUIs[rank].transform.DOPunchScale(Vector3.one * 0.5f, 0.2f).SetEase(Ease.OutQuad);
    }
}