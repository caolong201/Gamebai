using BestHTTP;
using BestHTTP.WebSocket;
using System;
using Suni.Enum;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Suni.Network
{
    public class ObservableProperty<T>
    {
        private T _value;
        public event Action<T> OnDataUpdated;

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    OnDataUpdated?.Invoke(_value);
                }
            }
        }
    }

    public enum ENetworkStatus
    {
        None,
        Connecting,
        Connected,
        LogOut
    }

    public class NetworkManager : SingletonMonoAwake<NetworkManager>
    {
        public ObservableProperty<bool> Register = new ObservableProperty<bool>();
        
        public ObservableProperty<JoinPhomGameRespone> JoinPhomGame = new ObservableProperty<JoinPhomGameRespone>();
        public ObservableProperty<EnterGameRespone> EnterGameRespone = new ObservableProperty<EnterGameRespone>();

        public ObservableProperty<OtherPlayerReadyRespone> PlayerReadyRespone =
            new ObservableProperty<OtherPlayerReadyRespone>();

        public ObservableProperty<StartPlayRespone> StartPlayRespone = new ObservableProperty<StartPlayRespone>();
        public ObservableProperty<PlayerLeftRespone> PlayerLeftRespone = new ObservableProperty<PlayerLeftRespone>();

        public ObservableProperty<PlayCardModelRespone> PlayCardModelRespone =
            new ObservableProperty<PlayCardModelRespone>();

        public ObservableProperty<PlayCardModelRespone> DrawFromDiscardRespone =
            new ObservableProperty<PlayCardModelRespone>();

        public ObservableProperty<PlayCardModelRespone> DrawFromDeckRespone =
            new ObservableProperty<PlayCardModelRespone>();

        public ObservableProperty<ResultRespone> ResultRespone = new ObservableProperty<ResultRespone>();
        public ObservableProperty<DropPhomRespone> DropPhomRespone = new ObservableProperty<DropPhomRespone>();
        public ObservableProperty<GuiBaiRespone> GuiBaiRespone = new ObservableProperty<GuiBaiRespone>();


        //long
        public ObservableProperty<ChatContentRespone> OnChatReceive = new ObservableProperty<ChatContentRespone>();

        public ENetworkStatus networkStatus = ENetworkStatus.None;
        private WebSocket m_webSocket = null;

        public Action OnConnected_Callback = null;
        public Action<string> OnReceivedMessage_Callback = null;


        public override void OnAwake()
        {
            base.OnAwake();
            JsonMapper.RegisterImporter<Int64, UInt64>((Int64 value) => { return (UInt64)value; });

            SetupNetwork();
        }

        private void SetupNetwork()
        {
            Debug.Log("SetupNetwork");
            ClearNetwork();

            OnConnected_Callback += async () =>
            {
                Debug.Log("On connected!!! Setup callback!!!");
                OnReceivedMessage_Callback += OnDataReceivedJson_Handle;
            };

            InitNetwork("ws://64.176.34.11:1357");
            //InitNetwork("wss://dev.sunisolution.com/ws");
            //InitNetwork("wss://ws-golden-dogs.suni.games");
        }

        private void OnDataReceivedJson_Handle(string _receivedMessage)
        {
            Debug.Log("Recieved Json: " + _receivedMessage);
            var respBase = JsonMapper.ToObject<BaseWebsocketRespone>(_receivedMessage);
            if (respBase.status != 200)
            {
                Debug.LogError("Lỗi " + respBase.error);
                UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(respBase.error));
                return;
            }

            if (respBase.currency != null)
            {
                if (respBase.currency.currentCoin > 0) GameManager.Instance.Coin = respBase.currency.currentCoin;
                if (respBase.currency.currentGold > 0) GameManager.Instance.Gold = respBase.currency.currentGold;
                if (respBase.currency.currentRuby > 0) GameManager.Instance.Ruby = respBase.currency.currentRuby;

                if (!string.IsNullOrEmpty(respBase.currency.message))
                {
                    if (respBase.currency.currentCoin > 0 || respBase.currency.currentGold > 0 ||
                        respBase.currency.currentRuby > 0)
                    {
                        // UIController.Instance.ShowConfirmModal("Chúc mừng", respBase.currency.message, "OK", "");
                        Debug.LogError("Chúc mừng " + respBase.currency.message);
                    }
                }
            }

            switch (respBase.eventType)
            {
                case (int)ENetworkHeader.Login: //1
                case (int)ENetworkHeader.LoginGuest: //2
                    var resp = JsonMapper.ToObject<LoginModelRespone>(_receivedMessage).data;
                    GameManager.Instance.NickName = resp.nickname;
                    GameManager.Instance.AvatarUrl = resp.avatarUrl;

                    string json =
                        JsonMapper.ToJson(new JoinPhomGameModel((int)ENetworkHeader.JoinPhomGame, (int)EGameType.PHOM));
                    SendJsonData(json);
                    break;
                case (int)ENetworkHeader.Register: //4
                    Register.Value = true;
                    break;
                case (int)ENetworkHeader.JoinPhomGame: //8
                    JoinPhomGame.Value = JsonMapper.ToObject<JoinPhomGameRespone>(_receivedMessage);
                    break;
                case (int)ENetworkHeader.EnterGame: //2000
                    if (SceneFader.Instance.CurrentScene == ESceneName.GamePlay)
                    {
                        EnterGameRespone.Value = JsonMapper.ToObject<EnterGameRespone>(_receivedMessage);
                    }
                    else
                    {
                        SceneFader.Instance.LoadScene(ESceneName.GamePlay,
                            () =>
                            {
                                DOVirtual.DelayedCall(0.05f,
                                    () =>
                                    {
                                        EnterGameRespone.Value =
                                            JsonMapper.ToObject<EnterGameRespone>(_receivedMessage);
                                    });
                            });
                    }

                    break;
                case (int)ENetworkHeader.PlayerLeft: //2002
                    PlayerLeftRespone.Value = JsonMapper.ToObject<PlayerLeftRespone>(_receivedMessage);
                    break;
                //case (int)ENetworkHeader.OtherPlayerReady: //2001
                case (int)ENetworkHeader.SelfReady: //2004
                    PlayerReadyRespone.Value = JsonMapper.ToObject<OtherPlayerReadyRespone>(_receivedMessage);
                    break;
                case (int)ENetworkHeader.StartGame: //2005
                case (int)ENetworkHeader.StartGame2: //2006 
                    StartPlayRespone.Value = JsonMapper.ToObject<StartPlayRespone>(_receivedMessage); // 
                    break;
                case (int)ENetworkHeader.PlayCard: //2007 
                    PlayCardModelRespone.Value = JsonMapper.ToObject<PlayCardModelRespone>(_receivedMessage); // 
                    break;
                case (int)ENetworkHeader.DrawFromDiscard: //2008 
                    DrawFromDiscardRespone.Value = JsonMapper.ToObject<PlayCardModelRespone>(_receivedMessage); // 
                    break;
                case (int)ENetworkHeader.DrawFromDeck: //2009 
                    DrawFromDeckRespone.Value = JsonMapper.ToObject<PlayCardModelRespone>(_receivedMessage); // 
                    break;
                case (int)ENetworkHeader.Result: //2010
                    ResultRespone.Value = JsonMapper.ToObject<ResultRespone>(_receivedMessage); // 
                    break;
                case (int)ENetworkHeader.DropPhom: //2011
                    DropPhomRespone.Value = JsonMapper.ToObject<DropPhomRespone>(_receivedMessage);
                    break;
                case (int)ENetworkHeader.GuiBai: //2012
                    Debug.LogError("GuiBai");
                    GuiBaiRespone.Value = JsonMapper.ToObject<GuiBaiRespone>(_receivedMessage);
                    break;


                //long
                case (int)ENetworkHeader.Chatcontent:
                    OnChatReceive.Value = JsonMapper.ToObject<ChatContentRespone>(_receivedMessage);

                    break;

            }
        }

        public void SendJsonData(string json)
        {
            Debug.Log("SendJsonData: " + json);
            m_webSocket.Send(json);
        }


        private void OnWebSocketOpen(WebSocket _webSocket)
        {
            Debug.Log("Success connected!!!");
            networkStatus = ENetworkStatus.Connected;
            OnConnected_Callback?.Invoke();
        }

        private void OnMessageReceived(WebSocket _webSocket, string message)
        {
            OnReceivedMessage_Callback?.Invoke(message);
        }


        private void OnWebSocketClosed(WebSocket _webSocket, ushort code, string message)
        {
            Debug.Log("OnWebSocketClosed");


            OnConnected_Callback = null;
            OnReceivedMessage_Callback = null;
            m_webSocket = null;

            networkStatus = ENetworkStatus.None;
        }

        private void OnWebSocketError(WebSocket _webSocket, string error)
        {
            Debug.Log(string.Format("OnWebSocketError!!! {0}", error));

            if (networkStatus == ENetworkStatus.Connecting) return;


            OnConnected_Callback = null;
            OnReceivedMessage_Callback = null;
            m_webSocket = null;

            networkStatus = ENetworkStatus.None;


            networkStatus = ENetworkStatus.Connecting;

            UIManager.Instance.ShowDialog(DialogName.UIMessageBox, new MessageBoxData(
                "Có vấn đề về kết nối!\n Vui lòng kiểm tra lại mạng và Đăng Nhập lại.", "OK",
                () =>
                {
                    SceneFader.Instance.LoadScene(ESceneName.Login);
                    SetupNetwork();
                }));
        }

        public void InitNetwork(string url)
        {
            m_webSocket = new WebSocket(new Uri(url));

#if UNITY_EDITOR
            m_webSocket.StartPingThread = true;
#if !BESTHTTP_DISABLE_PROXY
            if (null != HTTPManager.Proxy)
            {
                m_webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                    internalRequest.Proxy =
                        new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
            }
#endif
#endif
            m_webSocket.OnOpen += OnWebSocketOpen;
            m_webSocket.OnMessage += OnMessageReceived;
            m_webSocket.OnClosed += OnWebSocketClosed;
            m_webSocket.OnError += OnWebSocketError;

            m_webSocket.Open();
        }

        public void ClearNetwork()
        {
            OnConnected_Callback = null;
            OnReceivedMessage_Callback = null;

            if (null != m_webSocket)
            {
                Debug.Log("Close socket");
                m_webSocket.Close();
                m_webSocket = null;
            }
        }
    }
}