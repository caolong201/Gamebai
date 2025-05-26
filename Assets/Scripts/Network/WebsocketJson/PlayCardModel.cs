using System;
using System.Collections.Generic;

[Serializable]
public class PlayCardModel : BaseWebsocketRequest
{
    public PlayCardModelData data { get; set; }

    public PlayCardModel(int eventType, CardValue cardValue)
    {
        this.eventType = eventType;
        this.data = new PlayCardModelData()
        {
            card = cardValue
        };
    }
}

[Serializable]
public class PlayCardModelData
{
    public CardValue card { get; set; }
}

//resp
[Serializable]
public class PlayCardModelRespone : BaseWebsocketRespone
{
    public PlayCardModelResponeData data { get; set; }
}

[Serializable]
public class PlayCardModelResponeData
{
    public string nickname { get; set; }
    public string /*fromNickname*/ toNickname { get; set; }
    public CardValue card { get; set; }
    public int coinAmount { get; set; }
}