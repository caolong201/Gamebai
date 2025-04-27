
using System;
using System.Collections.Generic;

[Serializable]
public class LoginModel : BaseWebsocketRequest
{
    public LoginData data { get; set; }

    public LoginModel(int eventType, LoginData data) 
    {
        this.eventType = eventType;
        this.data = data;
    }
}

[Serializable]
public class LoginData
{
    public string username { get; set; }
    public string deviceId { get; set; }
    public int gameType { get; set; }
    public string version { get; set; }
}

//resp
[Serializable]
public class LoginModelRespone : BaseWebsocketRespone
{
    public LoginResponeData data { get; set; }
}

[Serializable]
public class LoginResponeData
{
    public string nickname { get; set; }
    public string avatarUrl { get; set; }
}


