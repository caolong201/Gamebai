

using System;
using System.Collections.Generic;

[Serializable]
public class DropPhomModel : BaseWebsocketRequest
{
    public DropPhomData data { get; set; }

    public DropPhomModel(int eventType, List<CardValue> cards) 
    {
        this.eventType = eventType;
        this.data = new DropPhomData()
        {
            cards = cards,
        };
    }
}

[Serializable]
public class DropPhomData
{
    public List<CardValue> cards { get; set; }
}

//resp
[Serializable]
public class DropPhomRespone : BaseWebsocketRespone
{
    public DropPhomResponeData data { get; set; }
}

[Serializable]
public class DropPhomResponeData
{
    public string nickname { get; set; }
    public List<CardValue> cards { get; set; }
}
