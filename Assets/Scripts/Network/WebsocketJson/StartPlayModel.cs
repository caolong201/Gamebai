
using System;
using System.Collections.Generic;

//resp
[Serializable]
public class StartPlayRespone : BaseWebsocketRespone
{
    public StartPlayResponeData data { get; set; }
}

[Serializable]
public class StartPlayResponeData
{
    public int drawPileCardCount { get; set; }
    public List<CardValue> playerCards { get; set; }
    public string firstTurn { get; set; }
}

[Serializable]
public class CardValue
{
    public int value { get; set; }
    public int type { get; set; }
}

