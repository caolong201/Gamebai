
using System;
using System.Collections.Generic;

[Serializable]
public class EnterGameModel : BaseWebsocketRequest
{
    public EnterGameData data { get; set; }

    public EnterGameModel(int eventType, int betAmount) 
    {
        this.eventType = eventType;
        this.data = new EnterGameData()
        {
            betAmount = betAmount,
            already = true
        };
    }
}

[Serializable]
public class EnterGameData
{
    public int betAmount { get; set; }
    public bool already { get; set; }
}

//resp
[Serializable]
public class EnterGameRespone : BaseWebsocketRespone
{
    public EnterGameResponeData data { get; set; }
}

[Serializable]
public class EnterGameResponeData
{
    public string id { get; set; }
    public int betAmount { get; set; }
    public string master { get; set; }
    public List<PlayerPosition> position { get; set; }
    public int status { get; set; }
}

public class PlayerPosition
{
    public string nickname { get; set; }
    public int position { get; set; }
    public bool isPlayer { get; set; }
}

