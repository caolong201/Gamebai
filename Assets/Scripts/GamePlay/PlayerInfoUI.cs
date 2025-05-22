using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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

    //money effects
    [SerializeField] private Transform moneyEffectRoot;
    [SerializeField] private GameObject bgWin, bgLose;
    [SerializeField] private TextMeshProUGUI txtMoneyEffect;

    //win eff
    [SerializeField] private GameObject winBG;
    [SerializeField] NicknameScroller nicknameScroller;

    //chat
    [SerializeField] private GameObject chatBubble;
    [SerializeField] private TextMeshProUGUI txtChat;
    [SerializeField] private Image imgChatIcon;
    public List<Sprite> iconSprites;
    [SerializeField] bool isFlipChatBubble = false;

    private PlayerPosition mInfo;
    private Tween hideChatTween;

    public void Init(PlayerPosition info)
    {
        mInfo = info;
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
        moneyEffectRoot.gameObject.SetActive(false);
        winBG.SetActive(false);

        if (nicknameScroller != null) nicknameScroller.Init();

        HideChat();
    }

    public void LoadRandomAvatar(string avatar)
    {
        string avatarName = "avatar_" + (string.IsNullOrEmpty(avatar) ? "0" : avatar);
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
            AudioManager.Instance.MomClip();
        }      
    }
    public void ShowRank(int rank, int winAmout)
    {
        if (imgMom.gameObject.activeSelf) return;
        if (rank >= rankUIs.Count) return;

        rankUIs[rank].SetActive(true);
        rankUIs[rank].transform.DOPunchScale(Vector3.one * 0.5f, 0.2f).SetEase(Ease.OutQuad);
        if (winAmout > 0)
        {
            winBG.SetActive(true);
            AudioManager.Instance.WinClip();
        }
        else
        {
            winBG.SetActive(false);          
        }
        ShowMoneyEffect(winAmout);
        mInfo.coin += winAmout;
        txtCoin.text = mInfo.coin.FormatCoins();
    }
    //ll
    public void ShowMoneyEffect(int money)
    {
        moneyEffectRoot.DOKill();
        moneyEffectRoot.gameObject.SetActive(true);
        moneyEffectRoot.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f).SetEase(Ease.OutQuad);
        if (money > 0)
        {
            bgWin.SetActive(true);
            bgLose.SetActive(false);
            txtMoneyEffect.text = money.FormatCoins();
            AudioManager.Instance.AddMoneyCoin();
        }
        else
        {
            bgWin.SetActive(false);
            bgLose.SetActive(true);
            txtMoneyEffect.text = "-" + (Mathf.Abs(money).FormatCoins());
            //AudioManager.Instance.Deductmoney();
        }
        DOVirtual.DelayedCall(3f, () => { moneyEffectRoot.gameObject.SetActive(false); });
    }
    public void ShowChat(string chatContent)
    {
        HideChat();
        if (string.IsNullOrEmpty(chatContent)) return;

        if (chatContent.StartsWith("@#$%_"))
        {
            // Xử lý icon
            string numberPart = chatContent.Replace("@#$%_", "");
            if (int.TryParse(numberPart, out int iconIndex))
            {
                if (iconIndex >= 0 && iconIndex < iconSprites.Count)
                {
                    imgChatIcon.sprite = iconSprites[iconIndex];
                    imgChatIcon.gameObject.SetActive(true);
                    imgChatIcon.SetNativeSize();

                    // Scale từ nhỏ đến lớn rồi trở lại bình thường
                    imgChatIcon.transform.DOKill();
                    imgChatIcon.transform.localScale = Vector3.one * 0.5f;
                    imgChatIcon.transform.DOScale(Vector3.one, 0.5f)
                        .SetEase(Ease.OutBounce)
                        .SetLoops(-1, LoopType.Yoyo);
                }
            }
        }
        else
        {
            txtChat.text = chatContent;
            chatBubble.SetActive(true);
            chatBubble.transform.localScale = Vector3.zero;
            chatBubble.GetComponent<VerticalLayoutGroup>().enabled = false;
            DOVirtual.DelayedCall(0.05f, () =>
            {
                chatBubble.GetComponent<VerticalLayoutGroup>().enabled = true;
                chatBubble.transform.DOKill();
                if (isFlipChatBubble)
                {
                    chatBubble.transform.DOScale(new Vector3(-1, 1, 1), 0.5f)
                        .SetEase(Ease.OutBounce);
                }
                else
                {
                    chatBubble.transform.DOScale(Vector3.one, 0.5f)
                        .SetEase(Ease.OutBounce);
                }
            });
        }

        if (hideChatTween != null) hideChatTween.Kill();
        hideChatTween = DOVirtual.DelayedCall(3f, HideChat).SetId(this);
    }

    private void HideChat()
    {
        if (chatBubble.activeSelf)
        {
            chatBubble.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    chatBubble.SetActive(false);
                    txtChat.text = string.Empty;
                });
        }


        imgChatIcon.gameObject.SetActive(false);
    }
}