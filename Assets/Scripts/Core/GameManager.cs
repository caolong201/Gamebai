using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoAwake<GameManager>
{
   
    public override void OnAwake()
    {
        base.OnAwake();
        Application.targetFrameRate = 60;
    }
    
    private float _gold = 0;
    public float Gold
    {
        set { _gold = value; }
        get { return _gold; }
    }

    private float _coin = 0;

    public float Coin
    {
        set { _coin = value; }
        get { return _coin; }
    }

    private float _ruby = 0;

    public float Ruby
    {
        set { _ruby = value; }
        get { return _ruby; }
    }

    private string _nickname = "";

    public string NickName
    {
        set { _nickname = value; }
        get { return _nickname; }
    }
    
    private string _avatarUrl = "";

    public string AvatarUrl
    {
        set { _avatarUrl = value; }
        get { return _avatarUrl; }
    }

    public bool IsMyself(string nickname)
    {
        return _nickname == nickname;
    }

    public string RoomMaster = "";

    public bool IsRoomMaster()
    {
        return IsMyself(RoomMaster);
    }


    private List<PlayerPosition> _players = new();

    public void AddPlayer(PlayerPosition player)
    {
        if (player == null) return;

        if (!_players.Contains(player))
        {
            _players.Add(player);
        }
    }

    public void RemovePlayer(PlayerPosition player)
    {
        foreach (var info in _players)
        {
            if (_players.Contains(player))
            {
                _players.Remove(player);
                break;
            }
        }
    }

    public int GetPlayersCount()
    {
        return _players.Count;
    }

    public List<PlayerPosition> Players
    {
        set { _players = value; }
        get { return _players; }
    }
}