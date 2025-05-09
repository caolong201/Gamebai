using System;
using System.Collections.Generic;

[Serializable]
public class GuiBaiModel : BaseWebsocketRequest
{
    public GuiBaiResponeData data { get; set; }

    public GuiBaiModel(int eventType, GuiBaiResponeData sendCards)
    {
        this.eventType = eventType;
        this.data = sendCards;
    }
}

//resp
[Serializable]
public class GuiBaiRespone : BaseWebsocketRespone
{
    public GuiBaiResponeData data { get; set; }
}

[Serializable]
public class GuiBaiResponeData
{
    public List<GuiBaiInfo> sendCards { get; set; }
}

[Serializable]
public class GuiBaiInfo
{
    public string fromNickname { get; set; }
    public string toNickname { get; set; }
    public List<CardValue> cards { get; set; }
}