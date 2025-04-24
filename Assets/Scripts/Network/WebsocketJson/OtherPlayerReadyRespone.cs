

using System;
using System.Collections.Generic;

[Serializable]
public class OtherPlayerReadyRespone : BaseWebsocketRespone
{
    public OtherPlayerReadyResponeData data { get; set; }
}

[Serializable]
public class OtherPlayerReadyResponeData
{
    public string nickname { get; set; }
}
