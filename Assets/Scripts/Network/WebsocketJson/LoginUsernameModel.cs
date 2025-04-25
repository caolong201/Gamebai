
using System;
using System.Collections.Generic;

[Serializable]
public class LoginUsernameModel : BaseWebsocketRequest
{
    public LoginUsernameModelData data { get; set; }

    public LoginUsernameModel(int eventType, LoginUsernameModelData data) 
    {
        this.eventType = eventType;
        this.data = data;
    }
}

[Serializable]
public class LoginUsernameModelData
{
    public string username { get; set; }
    public string password { get; set; }
    public int gameType { get; set; }
}

