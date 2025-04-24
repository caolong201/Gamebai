using System;

[Serializable]
public class BaseWebsocketRespone
{
    public int eventType { get; set; }
    public int status { get; set; }
    public string error { get; set; }
    public Currency currency { get; set; } = null;

}
[Serializable]
public class Currency
{
    public float currentCoin { get; set; } = 0;
    public float currentGold { get; set; } = 0;
    public float currentRuby { get; set; } = 0;
    public string message { get; set; } = string.Empty;

}

[Serializable]
public class BaseWebsocketRequest
{
    public int eventType { get; set; }

    public BaseWebsocketRequest()
    {
    }
    public BaseWebsocketRequest(int eventType)
    {
        this.eventType = eventType;
    }

}
