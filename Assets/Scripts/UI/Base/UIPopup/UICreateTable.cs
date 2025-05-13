using BestHTTP.JSON.LitJson;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using Suni.Enum;
using Suni.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class UICreateTable : GUIBaseDialogHandler
{
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private Button buttonMinus;
    [SerializeField] private Button buttonPlus;
    [SerializeField] GameObject imgTaoban;
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] textItems;
    public RectTransform centerZone; // vùng do
    public float snapSpeed = 5f;
    private bool isSnapping = false;
    private int[] betLevels = { 100, 500, 1000 }; // Các mức cược
    private int currentBetIndex = 0;
    private Action onCloseClick;
   
    public override void OnStart()
    {
        base.OnStart();
        imgTaoban.SetActive(false);
        buttonMinus.onClick.AddListener(DecreaseBet);
        buttonPlus.onClick.AddListener(IncreaseBet);
    }
    public override void OnBeginShow(object parameter)
    {
        base.OnBeginShow(parameter);
        imgTaoban.SetActive(true);

        RoomCreatorData data = parameter as RoomCreatorData;
        if (data != null)
        {
            onCloseClick = data.onCloseClick;        
        }

        UpdateBetText();
    }

    public override void OnEndHide(bool isDestroy)
    {
        base.OnEndHide(isDestroy);
        imgTaoban.SetActive(false);
    }

    private void Update()
    {
        if (!isSnapping && scrollRect.velocity.magnitude < 50f)
        {
            SnapToBestItem();
        }
    }
    private void IncreaseBet()
    {
        if (currentBetIndex < betLevels.Length - 1)
        {
            currentBetIndex++;
            UpdateBetText();
        }
    }
    private void DecreaseBet()
    {
        if (currentBetIndex > 0)
        {
            currentBetIndex--;
            UpdateBetText();
        }
    }
    private void UpdateBetText()
    {
        int bet = betLevels[currentBetIndex];
        betText.text = FormatBet(bet);
    }

    private string FormatBet(int bet)
    {
        return bet >= 1000 ? (bet / 1000) + "K" : bet.ToString();
    }
    public int GetSelectedBet()
    {
        return betLevels[currentBetIndex];
    }

    public void OnBtnCloseClicked()
    {
        onCloseClick?.Invoke();
        UIManager.Instance.HideDialog(DialogName.UICreateTable);
    }

    public void OnBtnOkClicked()
    {
        int selectedBet = GetSelectedBet();
        GameManager.Instance.Bet = selectedBet;
        string json = JsonMapper.ToJson(new EnterGameModel((int)ENetworkHeader.EnterGame, selectedBet));
        NetworkManager.Instance.SendJsonData(json);
    }
    private void SnapToBestItem()
    {
        Rect redRect = GetWorldRect(centerZone);
        float maxOverlap = -1f;
        RectTransform bestItem = null;

        foreach (var item in textItems)
        {
            Rect itemRect = GetWorldRect(item);
            float overlap = GetHorizontalOverlap(redRect, itemRect);

            if (overlap > maxOverlap)
            {
                maxOverlap = overlap;
                bestItem = item;
            }
        }

        if (bestItem != null)
        {
            StartCoroutine(SmoothSnap(bestItem));
        }
    }
    IEnumerator SmoothSnap(RectTransform targetItem)
    {
        isSnapping = true;

        Vector3 redCenter = centerZone.position;
        Vector3 itemCenter = targetItem.position;
        float diffX = redCenter.x - itemCenter.x;

        Vector2 startPos = content.anchoredPosition;
        float elapsed = 0f;
        float duration = 0.2f;
        Vector2 targetPos = startPos + new Vector2(diffX, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            content.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        content.anchoredPosition = targetPos;
        isSnapping = false;
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    private float GetHorizontalOverlap(Rect a, Rect b)
    {
        float left = Mathf.Max(a.xMin, b.xMin);
        float right = Mathf.Min(a.xMax, b.xMax);
        return Mathf.Max(0, right - left);
    }
}

public class RoomCreatorData
{
    public Action onCloseClick;
    public RoomCreatorData(Action _onOkClick = null)
    {
        onCloseClick = _onOkClick;
    }
    public RoomCreatorData(Action _onOkClick, Action _onCance)
    {
        onCloseClick = _onOkClick;
    }
}


