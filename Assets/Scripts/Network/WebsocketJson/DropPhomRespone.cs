

using System;
using System.Collections.Generic;

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
