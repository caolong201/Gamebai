using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabChatManager : MonoBehaviour
{
    public Button chatButton;
    public TMP_InputField chatInputField;
    public GameObject[] tabContents; 
    public Button[] tabButtons; 
    public Button[] iconButtons;
    public Button[] ChatButtons;
    [SerializeField] TMP_Text chatDisplayText;
    [SerializeField] GameObject showChat;

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
        chatButton.onClick.AddListener(OnChatButtonClicked);
        Init();
        ShowTab(0);
        InitItemIcon();
        InitItemChat();
    }
   public void OnChatButtonClicked()
    {
        chatInputField.gameObject.SetActive(true); // hiện input
        chatInputField.ActivateInputField();
    }
    public void Init()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => ShowTab(index));
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
       Debug.Log( index);
    }    
    public void OnbtnClickchat(int index)
    {

        if (index >= 0 && index < chatContents.Length)
        {
            chatDisplayText.text = chatContents[index]; 
            Debug.Log(chatContents[index]);              

        }
        else
        {
            chatDisplayText.text = "No content available.";
        }
    }
    public void CloseChat()
    {
        showChat.SetActive(false);
    }
}
