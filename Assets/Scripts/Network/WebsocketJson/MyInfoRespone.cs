

using System;
using System.Collections.Generic;

[Serializable]
public class MyInfoRespone : BaseWebsocketRespone
{
    public MyInfoResponeData data { get; set; }
}

[Serializable]
public class MyInfoResponeData
{
    public string nickname { get; set; }
}
