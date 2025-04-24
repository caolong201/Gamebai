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