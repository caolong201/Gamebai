using System;
using System.Collections.Generic;

[Serializable]
public class RegisterModel : BaseWebsocketRequest
{
    public RegisterModelData data { get; set; }

    public RegisterModel(int eventType, RegisterModelData data)
    {
        this.eventType = eventType;
        this.data = data;
    }
}

[Serializable]
public class RegisterModelData
{
    public string username { get; set; }
    public string password { get; set; }
    public int gameType { get; set; }
    public string nickname { get; set; }
}