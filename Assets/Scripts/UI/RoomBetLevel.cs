using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using Button = UnityEngine.UI.Button;

public class RoomBetLevel : MonoBehaviour
{
    public GameObject btnPrefab;
    public Transform content;
    [SerializeField] RectTransform footer;
    private List<RoomTableInfo> roomDatas;
    RomItem romclick;
    [SerializeField] GameObject imgTaoban;
    [SerializeField] GameObject bntBack;
    private RoomHUD roomHUD;

    void Start()
    {
        bntBack.SetActive(true);
        roomDatas = NetworkManager.Instance.JoinPhomGame.Value.data.roomList;

        roomHUD = FindObjectOfType<RoomHUD>();
        if (roomHUD == null)
        {
            Debug.LogError("RoomHUD not found!");
        }


        footer.anchoredPosition = new Vector2(0, -150);
        footer.DOAnchorPosY(88, .8f).SetEase(Ease.OutCirc);

        for (int i = 0; i < roomDatas.Count; i++)
        {
            GameObject btnObj = Instantiate(btnPrefab, content);
            RomItem bet = btnObj.GetComponent<RomItem>();
            if (bet != null)
            {
                bet.Init(roomDatas[i], OnRoomItemClicked);
            }
        }
    }
    private void OnRoomItemClicked(RoomTableInfo selectedRoom)
    {
        Debug.Log("Clicked room with bet amount: " + selectedRoom.betAmount);


        GameManager.Instance.Bet = selectedRoom.betAmount;
        string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, selectedRoom.betAmount));
        NetworkManager.Instance.SendJsonData(json);

    }

    public void bntTaoban()
    {
        UIManager.Instance.ShowDialog(DialogName.UICreateTable, new RoomCreatorData(
               () =>
               {
                   Debug.Log("taoban");

               }));
    }

    public void bntVaoban()
    {
        GUIDialogBase dlg = UIManager.Instance.ShowDialog(DialogName.UIEntertable, new EntertableData(() =>
        {
            Debug.Log("Voban");
        }));
        if (dlg != null)
        {
            Transform contentToScale = dlg.transform.Find("dialogContent");
            if (contentToScale != null)
            {
                contentToScale.localScale = Vector3.zero;
                contentToScale.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }
        }
    }

    public void OnbtBackLogin()
    {

        this.gameObject.SetActive(false);
        if (roomHUD != null)
        {
            roomHUD.ShowMainUI();
        }
        bntBack.SetActive(false);

    }
}




