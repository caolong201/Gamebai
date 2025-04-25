using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.JSON.LitJson;
using UnityEngine;
using DG.Tweening;
using Suni.Enum;
using Suni.Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhomGameManager : MonoBehaviour
{
    [SerializeField] GamePlayHUD gamePlayHUD;

    public Transform deckPosition; // Vị trí bộ bài
    public Transform deckCardCountObject;
    
    public List<Player> playerHands; // Danh sách người chơi
    public Transform playerHandArea; // Khu vực bài của Player
    public GameObject cardPrefab; // Prefab lá bài
    public Transform[] discardPilePositions; // Vị trí discard pile của mỗi người chơi

    private List<Card> deck = new List<Card>(); // Danh sách bộ bài

    private List<Stack<Card>> discardPiles = new List<Stack<Card>>(); // Danh sách discard pile của mỗi người chơi

    //private string[] suits = { "♠", "♥", "♦", "♣" };
    private string[] suits = { "♥", "♦", "♣", "♠" };
    private List<CardValue> myCardValues;
    private int cardsPerPlayer = 10; // Số lượng bài mỗi người chơi nhận
    private float cardSpacing = 125; // Khoảng cách giữa các lá bài của Player

    private int currentPlayerIndex = 0; // Chỉ số người chơi hiện tại
    private Vector3 playerTargetPosition;

    private bool isFirstRound = false;
    public bool isCanSelectCard = false;
    
    //drop phom
    public Transform[] dropPhomPositions;
    private DropPhomRespone haPhomData;

    //result - show all cards
    public Transform[] showAllCardsPositions;

    [SerializeField] private TextMeshProUGUI txtdrawPileCardCount;
    private int drawPileCardCount = 0;
    
    private bool isEndGame = false;

    private void Start()
    {
        NetworkManager.Instance.EnterGameRespone.OnDataUpdated += EnterGameRespone;
        NetworkManager.Instance.StartPlayRespone.OnDataUpdated += StartPlayRespone;//2006
        //NetworkManager.Instance.PlayerReadyRespone.OnDataUpdated += PlayerReadyRespone;
        NetworkManager.Instance.PlayerLeftRespone.OnDataUpdated += PlayerLeftRespone; //2002
        NetworkManager.Instance.PlayCardModelRespone.OnDataUpdated += PlayCardModelRespone; //2007
        NetworkManager.Instance.DrawFromDiscardRespone.OnDataUpdated += DrawFromDiscardRespone; //2008
        NetworkManager.Instance.DrawFromDeckRespone.OnDataUpdated += DrawFromDeckRespone; //2009
        NetworkManager.Instance.ResultRespone.OnDataUpdated += ResultRespone; //2010
        NetworkManager.Instance.DropPhomRespone.OnDataUpdated += DropPhomRespone; //2011

        foreach (var playerHands in playerHands)
        {
            playerHands.gameObject.SetActive(false);
        }

        isEndGame = false;
        txtdrawPileCardCount.text = "";
        deckCardCountObject.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.IsInstanceValid())
        {
            NetworkManager.Instance.EnterGameRespone.OnDataUpdated -= EnterGameRespone;
            NetworkManager.Instance.StartPlayRespone.OnDataUpdated -= StartPlayRespone;
            //NetworkManager.Instance.PlayerReadyRespone.OnDataUpdated -= PlayerReadyRespone;
            NetworkManager.Instance.PlayerLeftRespone.OnDataUpdated -= PlayerLeftRespone;
            NetworkManager.Instance.PlayCardModelRespone.OnDataUpdated -= PlayCardModelRespone;
            NetworkManager.Instance.DrawFromDiscardRespone.OnDataUpdated -= DrawFromDiscardRespone;
            NetworkManager.Instance.DrawFromDeckRespone.OnDataUpdated -= DrawFromDeckRespone;
            NetworkManager.Instance.ResultRespone.OnDataUpdated -= ResultRespone; //2010
            NetworkManager.Instance.DropPhomRespone.OnDataUpdated -= DropPhomRespone; //2011
        }
    }

    private void ArrangeSeats(float delay = 0)
    {
        foreach (var playerHands in playerHands)
        {
            playerHands.gameObject.SetActive(false);
        }

        Debug.Log("ArrangeSeats");
        var seats = GameManager.Instance.Players;
        foreach (var seat in seats)
        {
            playerHands[seat.position].gameObject.SetActive(true);
            playerHands[seat.position].Init(seat);
            Debug.LogError(seat.nickname + " # seat: " + seat.position);
        }
    }

    #region Event BE

    private void ResultRespone(ResultRespone obj)
    {
        if (isEndGame) return;

        DOVirtual.DelayedCall(1, () =>
        {
            for (int i = 0; i < obj.data.winArray.Count; i++)
            {
                var player = FindPlayer(obj.data.winArray[i].nickname);
                if (player == null || obj.data.winArray[i].cards == null || obj.data.winArray[i].cards.Count == 0)
                {
                    continue;
                }

                player.inforUI.ShowRank(i);

                if (GameManager.Instance.IsMyself(obj.data.winArray[i].nickname))
                    continue;

                var listCards = obj.data.winArray[i].cards;
                int countCards = listCards.Count;

                for (int j = 0; j < countCards; j++)
                {
                    Card card = null;
                    if (player.GetHand().Count <= 0)
                    {
                        GameObject cardObj = Instantiate(cardPrefab,
                            playerHands[player.seatInfo.position].transform.position, Quaternion.identity);
                        card = cardObj.GetComponent<Card>();
                        card.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        card = player.GetHand()[0];
                        player.GetHand().Remove(card);
                    }

                    card.SetCard(listCards[j].value, suits[listCards[j].type - 1]);
                    card.Up();
                    card.transform.SetParent(showAllCardsPositions[player.seatInfo.position]);
                    card.transform.localScale = Vector3.one;
                }

                showAllCardsPositions[player.seatInfo.position].GetComponent<LayoutGroup>().enabled = true;
            }

            isEndGame = true;
            gamePlayHUD.ShowXepBai(false);


            DOVirtual.DelayedCall(5, () =>
            {
              //Clear all
                var cards = transform.GetComponentsInChildren<Card>(true);
                for (int i = cards.Length - 1; i >= 0; i--) // Backward iteration to avoid issues
                {
                    if (cards[i] != null)
                    {
                        if (Application.isPlaying)
                            Destroy(cards[i].gameObject);
                        else
                            DestroyImmediate(cards[i].gameObject);
                    }
                }

                deck.Clear();
                discardPiles.Clear();
                myCardValues.Clear();
                isEndGame = false;
                gamePlayHUD.ResetUI();

                ArrangeSeats();

                if (GameManager.Instance.GetPlayersCount() >= 2 && GameManager.Instance.IsRoomMaster())
                {
                    gamePlayHUD.ShowChiaBai(true);
                }
            });
        });
    }

    private void DropPhomRespone(DropPhomRespone obj)
    {
        var player = FindPlayer(obj.data.nickname);
        if (GameManager.Instance.IsMyself(obj.data.nickname))
        {
            haPhomData = obj;
            if (player != null && haPhomData != null)
            {
                if (haPhomData.data.cards == null || haPhomData.data.cards.Count == 0)
                {
                    player.inforUI.ShowMom();
                    return;
                }
            }

            gamePlayHUD.ShowHaPhom(true);
        }
        else
        {
            if (player != null && haPhomData != null)
            {
                if (haPhomData.data.cards == null || haPhomData.data.cards.Count == 0)
                {
                    player.inforUI.ShowMom();
                    return;
                }

                var phoms = PhomChecker.FindPhoms(haPhomData.data.cards);
                float startY = 0f;
                RectTransform rt;
                foreach (var phom in phoms)
                {
                    float startX = -(phom.Count + (phom.Count - 1) * cardSpacing) / 2f;

                    // Tạo từng lá bài trong phỏm
                    for (int i = 0; i < phom.Count; i++)
                    {
                        Card card = player.GetHand()[0];
                        rt = card.GetComponent<RectTransform>();
                        card.SetCard(phom[i].value, suits[phom[i].type - 1]);
                        card.Up();
                        player.GetHand().Remove(card);
                        card.transform.SetParent(dropPhomPositions[player.seatInfo.position]);

                        // Tính vị trí thủ công
                        Vector2 target = new Vector2(
                            startX + i * (cardSpacing / 1.5f),
                            startY
                        );
                        rt.DOAnchorPos(target, 0.3f);
                    }

                    // Xuống dòng cho phỏm tiếp theo
                    startY -= 50;
                }
            }
        }
    }

    private void DrawFromDeckRespone(PlayCardModelRespone obj)
    {
        Card drawnCard = DrawFromDeck();
        if (GameManager.Instance.IsMyself(obj.data.nickname))
        {
            if (drawnCard != null)
            {
                drawnCard.SetCard(obj.data.card.value, suits[obj.data.card.type - 1]);
                playerHands[0].AddCardToHand(drawnCard);

                playerTargetPosition += new Vector3(cardSpacing, 0, 0);
                drawnCard.transform.DOLocalMove(playerTargetPosition, 0.3f).SetEase(Ease.OutQuad);
                drawnCard.Up();
                drawnCard.transform.localScale = Vector3.one;
                drawnCard.AddComponent<Button>().onClick.AddListener(() => OnCardClicked(drawnCard));
            }
        }
        else
        {
            //other players see
            if (drawnCard != null)
            {
                drawnCard.transform.DOMove(FindPlayer(obj.data.nickname).transform.position, 0.3f)
                    .SetEase(Ease.OutQuad);
                drawnCard.transform.localScale = Vector3.one;
            }
        }

        drawPileCardCount -= 1;
        txtdrawPileCardCount.text = drawPileCardCount + "";
    }

    private void DrawFromDiscardRespone(PlayCardModelRespone obj)
    {
        if (GameManager.Instance.IsMyself(obj.data.nickname)) return;

        int previousPlayerIndex = GetPreviousPlayerIndex(currentPlayerIndex);

        Debug.Log("previousPlayerIndex: " + previousPlayerIndex + " # currentPlayerIndex: " + currentPlayerIndex);
        Stack<Card> previousDiscardPile = discardPiles[previousPlayerIndex];

        if (previousDiscardPile.Count > 0)
        {
            Card topDiscard = previousDiscardPile.Pop();
            topDiscard.transform.SetParent(FindPlayer(obj.data.nickname).transform);
            topDiscard.transform.DOLocalMove(new Vector3(0, -cardSpacing / 3, 0), 0.3f)
                .SetEase(Ease.OutQuad);
            topDiscard.transform.localScale = Vector3.one;
            topDiscard.ShowEffect(true);
            playerHands[currentPlayerIndex].AddCardToHand(topDiscard);
        }
    }

    private void PlayCardModelRespone(PlayCardModelRespone obj)
    {
        if (GameManager.Instance.IsMyself(obj.data.nickname)) return;

        //handle only for other players
        var player = FindPlayer(obj.data.nickname);
        if (player != null)
        {
            Card card = null;
            if (player.GetHand().Count <= 0)
            {
                GameObject cardObj = Instantiate(cardPrefab,
                    playerHands[player.seatInfo.position].transform.position, Quaternion.identity);
                card = cardObj.GetComponent<Card>();
                card.transform.localScale = Vector3.one;
            }
            else
            {
                card = player.GetHand()[0];
            }

            card.SetCard(obj.data.card.value, suits[obj.data.card.type - 1]);
            DiscardCard(player, card);
            NextTurn();
        }
    }

    private void EnterGameRespone(EnterGameRespone obj)
    {
        GameManager.Instance.RoomMaster = obj.data.master;
        GameManager.Instance.Players = obj.data.position;
        ArrangeSeats();
    }

    private void StartPlayRespone(StartPlayRespone obj)
    {
        drawPileCardCount = obj.data.drawPileCardCount;
        isFirstRound = true;
        myCardValues = obj.data.playerCards;
        foreach (var playerHands in playerHands)
        {
            if (!playerHands.gameObject.activeSelf) continue;

            if (playerHands.seatInfo.nickname == obj.data.firstTurn)
            {
                currentPlayerIndex = playerHands.seatInfo.position;
                break;
            }
        }

        ChiaBai();
    }

    private void PlayerReadyRespone(OtherPlayerReadyRespone obj)
    {
        // if (string.IsNullOrEmpty(obj.data.nickname)) return;
        //
        // GameManager.Instance.AddPlayer(obj.data.nickname);
        //
        // ArrangeSeats();
    }

    private void PlayerLeftRespone(PlayerLeftRespone obj)
    {
        GameManager.Instance.RoomMaster = obj.data.newMatser;
        GameManager.Instance.Players = obj.data.position;
        ArrangeSeats();
    }

    #endregion Event BE

    public void ChiaBai()
    {
        
        playerHands[0].inforUI.transform.DOLocalMoveX(-616, 0.4f).SetEase(Ease.OutQuad);
        InitializeDeck();
        InitializeDiscardPiles();
        StartCoroutine(DealCards(() =>
        {
            gamePlayHUD.ShowXepBai(true);
            isCanSelectCard = true;
            deckCardCountObject.gameObject.SetActive(true);
            deckCardCountObject.SetAsLastSibling();
            StartTurn();
        }));
    }

    // Khởi tạo bộ bài
    void InitializeDeck()
    {
        foreach (string suit in suits)
        {
            for (int value = 1; value <= 13; value++)
            {
                GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity);
                Card card = cardObj.GetComponent<Card>();
                card.SetCard(value, suit);
                card.transform.SetParent(deckPosition);
                card.transform.localScale = Vector3.one;
                deck.Add(card);
            }
        }

        txtdrawPileCardCount.text = drawPileCardCount + "";
    }

    // Xáo trộn bộ bài
    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randIndex = Random.Range(0, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randIndex];
            deck[randIndex] = temp;
        }
    }

    // Khởi tạo discard pile cho mỗi người chơi
    void InitializeDiscardPiles()
    {
        for (int i = 0; i < playerHands.Count; i++)
        {
            discardPiles.Add(new Stack<Card>());
        }
    }

    // Chia bài cho người chơi
    IEnumerator DealCards(System.Action completion = null)
    {
        for (int i = 0; i < cardsPerPlayer; i++)
        {
            for (int j = 0; j < playerHands.Count; j++)
            {
                if (playerHands[j].gameObject.activeSelf == false) continue;
                if (deck.Count == 0) yield break;

                Card card = deck[0];
                deck.RemoveAt(0);

                card.transform.SetParent(playerHands[j].transform);

                if (j == 0) // Nếu là Player, xếp bài thành hàng ngang
                {
                    if (i >= myCardValues.Count)
                    {
                        Destroy(card.gameObject);
                        continue;
                    }

                    float xOffset = i * cardSpacing;
                    playerTargetPosition = playerHandArea.localPosition + new Vector3(xOffset, 0, 0);
                    card.SetCard(myCardValues[i].value, suits[myCardValues[i].type - 1]);
                    card.transform.DOLocalMove(playerTargetPosition, 0.3f).SetEase(Ease.OutQuad);
                    card.Up(); // Ngửa bài lên cho Player
                    card.transform.localScale = Vector3.one;
                    card.AddComponent<Button>().onClick.AddListener(() => OnCardClicked(card));
                }
                else // Nếu là other player, bài vẫn úp xuống
                {
                    card.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutQuad);
                }

                playerHands[j].AddCardToHand(card);

                yield return new WaitForSeconds(0.02f);
            }
        }

        completion?.Invoke();
    }

    // Bắt đầu lượt chơi
    private void StartTurn()
    {
        foreach (var player in playerHands)
        {
            player.inforUI.StopTimer();
        }

        Debug.LogError("StartTurn: " + currentPlayerIndex);
        Player currentPlayer = playerHands[currentPlayerIndex];
        currentPlayer.inforUI.StartTimer();

        if (currentPlayerIndex == 0) // my turn
        {
            EnablePlayerControls(true);
        }
        else
        {
            EnablePlayerControls(false);
        }
    }

    // Đánh lá bài vào discard pile
    private void DiscardCard(Player player, Card card)
    {
        // Xóa lá bài khỏi tay người chơi
        player.RemoveCardFromHand(card);

        // Thêm lá bài vào discard pile của người chơi hiện tại
        discardPiles[currentPlayerIndex].Push(card);

        // Đặt lá bài làm con của discardPilePositions
        card.transform.SetParent(discardPilePositions[currentPlayerIndex]);

        // Tính toán vị trí mới cho lá bài dựa trên số lượng lá bài trong discard pile
        int cardCount = discardPiles[currentPlayerIndex].Count;
        float cardWidth = 100f; // Chiều rộng của mỗi lá bài (có thể điều chỉnh)
        float spacing = 10f; // Khoảng cách giữa các lá bài (có thể điều chỉnh)

        // Tính toán vị trí x mới
        float startX = ((cardCount - 1) * (cardWidth + spacing)) / 2f; // Canh giữa các lá bài
        //Vector3 newPosition = discardPilePositions[currentPlayerIndex].localPosition +  new Vector3(startX + (cardCount - 1) * (cardWidth + spacing), 0, 0);
        Debug.Log(startX);
        // Di chuyển lá bài đến vị trí mới với animation
        card.transform.DOLocalMove(new Vector3(startX, 0, 0), 0.3f).SetEase(Ease.OutQuad);

        // Ngửa lá bài lên
        card.Up();
    }

    // Rút lá bài từ nọc
    public Card DrawFromDeck()
    {
        if (deck.Count > 0)
        {
            Card card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        return null;
    }

    // Chuyển lượt chơi sang người tiếp theo
    public void NextTurn()
    {
        isCanSelectCard = isFirstRound;
        currentPlayerIndex = GetNextPlayerIndex(currentPlayerIndex);
        StartTurn();
    }

    public int GetNextPlayerIndex(int currIndex)
    {
        for (int i = 1; i <= playerHands.Count; i++)
        {
            int nextIndex = (currIndex + i) % 4;
            Player nextPlayer = playerHands.Find(p => p.gameObject.activeSelf && p.seatInfo.position == nextIndex);

            if (nextPlayer != null)
                return nextIndex;
        }

        Debug.LogWarning("Không tìm thấy người chơi tiếp theo hợp lệ!");
        return currIndex;
    }


    public int GetPreviousPlayerIndex(int currIndex)
    {
        Debug.LogError("currIndex: " + currIndex);
        for (int i = 1; i <= playerHands.Count; i++)
        {
            int prevIndex = (currIndex - i + 4) % 4; // +4 để tránh số âm
            Debug.LogError("prevIndex: " + prevIndex);
            Player prevPlayer =
                playerHands.Find(p => p != null && p.gameObject.activeSelf && p.seatInfo.position == prevIndex);

            if (prevPlayer != null)
                return prevIndex;
        }

        Debug.LogWarning("Không tìm thấy người chơi trước đó hợp lệ!");
        return currIndex;
    }

    // Kích hoạt điều khiển cho người chơi
    private void EnablePlayerControls(bool isEnable)
    {
        if (!isEnable)
        {
            gamePlayHUD.ShowRutBai(false);
            gamePlayHUD.ShowAnBai(false);
            gamePlayHUD.ShowDanhBai(false);
        }
        else
        {
            if (!isFirstRound)
            {
                gamePlayHUD.ShowRutBai(true);

                int previousPlayerIndex = GetPreviousPlayerIndex(currentPlayerIndex);
                Stack<Card> previousDiscardPile = discardPiles[previousPlayerIndex];
                if (previousDiscardPile.Count > 0)
                {
                    Card topDiscard = previousDiscardPile.Peek();
                    if (PhomChecker.CanFormPhom(playerHands[0].GetHand(), topDiscard))
                    {
                        gamePlayHUD.ShowAnBai(true);
                    }
                }
            }
        }

        isFirstRound = false;
    }

    // Sắp xếp bài của người chơi

    #region XepBai

    public void SortPlayerCards()
    {
        Player myPlayer = playerHands[0];
        var sortedCards = PhomChecker.SortCards(myPlayer.GetHand());
        foreach (var card in sortedCards)
        {
            Debug.Log(card.value + " : " + card.suit + " pos: ");
        }
        myPlayer.SetHand(sortedCards);
        SapXepViTriBai();
    }

    #endregion

    #region Player control

    private Card selectedCard;

    // Xử lý khi người chơi rút bài từ nọc
    public void OnDrawFromDeck()
    {
        string json = JsonMapper.ToJson(new BaseWebsocketRequest((int)ENetworkHeader.DrawFromDeck));
        NetworkManager.Instance.SendJsonData(json);
    }

    // Xử lý khi người chơi rút bài từ discard pile
    public void OnDrawFromDiscard()
    {
        int previousPlayerIndex = GetPreviousPlayerIndex(currentPlayerIndex);

        Debug.Log("previousPlayerIndex: " + previousPlayerIndex + " # currentPlayerIndex: " + currentPlayerIndex);
        Stack<Card> previousDiscardPile = discardPiles[previousPlayerIndex];

        if (previousDiscardPile.Count > 0)
        {
            Card topDiscard = previousDiscardPile.Pop();
            playerHands[0].AddCardToHand(topDiscard);

            // Animation di chuyển lá bài từ discard pile đến tay người chơi
            playerTargetPosition += new Vector3(cardSpacing, 0, 0);
            topDiscard.transform.DOLocalMove(playerTargetPosition, 0.3f).SetEase(Ease.OutQuad);
            topDiscard.transform.localScale = Vector3.one;
            topDiscard.AddComponent<Button>().onClick.AddListener(() => OnCardClicked(topDiscard));
            topDiscard.ShowEffect(true);

            string json = JsonMapper.ToJson(new PlayCardModel((int)ENetworkHeader.DrawFromDiscard, new CardValue()
            {
                value = topDiscard.value,
                type = PhomChecker.GetSuitNumber(topDiscard.suit)
            }));
            NetworkManager.Instance.SendJsonData(json);
        }
    }

    // Xử lý khi người chơi chọn lá bài
    private void OnCardSelected(Card card)
    {
        if (selectedCard == card) return;

        if (selectedCard != null)
        {
            // Đưa lá bài đã chọn trước đó về vị trí ban đầu
            selectedCard.transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.OutQuad);
        }

        // Chọn lá bài mới và di chuyển nó lên trên
        selectedCard = card;
        selectedCard.transform.DOLocalMoveY(selectedCard.transform.localPosition.y + 50, 0.1f).SetEase(Ease.OutQuad);

        gamePlayHUD.ShowDanhBai(true);
    }

    // Xử lý khi người chơi nhấn nút "Đánh bài"
    public void OnDiscardCard()
    {
        if (selectedCard != null)
        {
            string json = JsonMapper.ToJson(new PlayCardModel((int)ENetworkHeader.PlayCard, new CardValue()
            {
                value = selectedCard.value,
                type = PhomChecker.GetSuitNumber(selectedCard.suit)
            }));
            NetworkManager.Instance.SendJsonData(json);

            // Đánh lá bài đã chọn
            DiscardCard(playerHands[0], selectedCard);

            // Reset lá bài được chọn
            selectedCard = null;

            SapXepViTriBai();

            // Chuyển lượt chơi sang người tiếp theo
            NextTurn();
        }
        else
        {
            Debug.Log("Vui lòng chọn lá bài để đánh!");
        }
    }

    private void SapXepViTriBai()
    {
        for (int i = 0; i < playerHands[0].GetHand().Count; i++)
        {
            Vector3 targetPosition = playerHandArea.localPosition + new Vector3(i * cardSpacing, 0, 0);
            playerHands[0].GetHand()[i].transform.SetSiblingIndex(i);
            playerHands[0].GetHand()[i].transform.DOLocalMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);

            playerTargetPosition = targetPosition;
        }
    }

    private void OnCardClicked(Card card)
    {
        if (currentPlayerIndex != 0 || !isCanSelectCard || isEndGame) return;
        Debug.Log("OnCardClicked: " + card.value);
        OnCardSelected(card);
    }

    #endregion

    private Player FindPlayer(string nickname)
    {
        foreach (var player in playerHands)
        {
            if (player.gameObject.activeSelf && player.seatInfo.nickname == nickname)
            {
                return player;
            }
        }

        return null;
    }

    public void HaPhom()
    {
        var player = FindPlayer(haPhomData.data.nickname);
        if (player != null && haPhomData != null)
        {
            if (haPhomData.data.cards == null || haPhomData.data.cards.Count == 0)
            {
                player.inforUI.ShowMom();
                return;
            }

            var phoms = PhomChecker.FindPhoms(haPhomData.data.cards);
            float startY = 0f;
            RectTransform rt;
            foreach (var phom in phoms)
            {
                float startX = -(phom.Count + (phom.Count - 1) * cardSpacing) / 2f;

                // Tạo từng lá bài trong phỏm
                for (int i = 0; i < phom.Count; i++)
                {
                    Card card = player.GetHand().Find(x =>
                        x.value == phom[i].value && PhomChecker.GetSuitNumber(x.suit) == phom[i].type);
                    if (card == null)
                    {
                        Debug.LogError("could not found: " + phom[i].value + " : " + phom[i].type);
                        return;
                    }

                    rt = card.GetComponent<RectTransform>();
                    player.GetHand().Remove(card);
                    card.transform.SetParent(dropPhomPositions[player.seatInfo.position]);

                    // Tính vị trí thủ công
                    Vector2 target = new Vector2(
                        startX + i * (cardSpacing / 1.5f),
                        startY
                    );
                    rt.DOAnchorPos(target, 0.3f);
                }

                // Xuống dòng cho phỏm tiếp theo
                startY -= 50;
            }
        }
    }
}