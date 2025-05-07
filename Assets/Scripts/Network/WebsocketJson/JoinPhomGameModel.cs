using System;
using System.Collections.Generic;

[Serializable]
public class JoinPhomGameModel : BaseWebsocketRequest
{
    public JoinPhomGameData data { get; set; }

    public JoinPhomGameModel(int eventType, int gameType)
    {
        this.eventType = eventType;
        this.data = new JoinPhomGameData()
        {
            gameType = gameType
        };
    }
}

[Serializable]
public class JoinPhomGameData
{
    public int gameType { get; set; }
}

//resp
[Serializable]
public class JoinPhomGameRespone : BaseWebsocketRespone
{
    public JoinPhomGameResponeData data { get; set; }
}

[Serializable]
public class JoinPhomGameResponeData
{
    public List<RoomTableInfo> roomList { get; set; }
}

[Serializable]
public class RoomTableInfo
{
    public int betAmount { get; set; }
    public int clientCount { get; set; }
}
