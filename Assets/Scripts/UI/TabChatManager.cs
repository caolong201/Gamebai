using System;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabChatManager : MonoBehaviour
{
    public TMP_InputField chatInputField;
    public GameObject[] tabContents;
    public Button[] tabButtons;
    public Button[] iconButtons;
    public Button[] ChatButtons;

    public RectTransform panelParent;
    public GameObject bntChat;

    private string[] chatContents = new string[]
    {
        "Đánh lẹ đi pa",
        "Cho xin cây chốt",
        "Sắp ù rồi",
        "Ăn nhiều thế",
        "Móm nè",
        "Giờ thì ăn đi",
        "Đen vãi hàng",
        "Chơi khô máu luôn",
        "Thắng rồi yeah yeah",
    };

    void Start()
    {
        InitTabs();

        ShowTab(0);
        InitItemIcon();
        InitItemChat();
        UpdateTabButtonAlpha(0);
    }

    private void InitTabs()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                ShowTab(index);
                UpdateTabButtonAlpha(index);
            });
        }
    }

    private void UpdateTabButtonAlpha(int activeIndex)
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            float targetAlpha = (i == activeIndex) ? 1f : 0.35f;
            Image img = tabButtons[i].GetComponent<Image>();
            if (img != null)
            {
                img.DOFade(targetAlpha, 0.2f);
            }
        }
    }


    public void ShowTab(int index)
    {
        for (int i = 0; i < tabContents.Length; i++)
        {
            tabContents[i].SetActive(i == index);
            Debug.Log($"Tab {i} active: {i == index}");
        }
    }

    public void InitItemIcon()
    {
        for (int i = 0; i < iconButtons.Length; i++)
        {
            int index = i;
            iconButtons[i].onClick.AddListener(() => OnIconClicked(index));
        }
    }

    public void InitItemChat()
    {
        for (int i = 0; i < ChatButtons.Length; i++)
        {
            int index = i;
            ChatButtons[i].onClick.AddListener(() => OnbtnClickchat(index));
        }
    }

    public void OnIconClicked(int index)
    {
        string iconId = $"@#$%_{index}";
        string json = JsonMapper.ToJson(new ChatContentModel((int)ENetworkHeader.Chatcontent, new ChatContentModelData()
        {
            data = new ChatContentModelData2()
            {
                chatContent = iconId    ,
                nickname = GameManager.Instance.NickName ,
            }

        })) ;
        NetworkManager.Instance.SendJsonData(json);
        CloseChat();
    }
    public void OnbtnClickchat(int index)
    {
        if (index >= 0 && index < chatContents.Length)
        {       
            string content = chatContents[index];
            string json = JsonMapper.ToJson(new ChatContentModel((int)ENetworkHeader.Chatcontent, new ChatContentModelData()
            {
                data = new ChatContentModelData2()
                {
                    chatContent = content    ,
                    nickname = GameManager.Instance.NickName,
                }
            }));        
            NetworkManager.Instance.SendJsonData(json);
            CloseChat();
        }
    }

    public void OnEndEdit()
    {
        if (chatInputField != null && chatInputField.text != "")
        {
            string content = chatInputField.text;
            string json = JsonMapper.ToJson(new ChatContentModel((int)ENetworkHeader.Chatcontent, new ChatContentModelData()
            {
                data = new ChatContentModelData2()
                {
                    chatContent = content    ,
                    nickname = GameManager.Instance.NickName,
                }
            }));        
            NetworkManager.Instance.SendJsonData(json);
            chatInputField.text = string.Empty;
            CloseChat();
        }
    }

    public void CloseChat()
    {
        bntChat.SetActive(true);
        panelParent.anchoredPosition = new Vector2(500, 625);
    }


}