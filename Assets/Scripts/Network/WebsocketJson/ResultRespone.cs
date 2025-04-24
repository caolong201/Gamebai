

using System;
using System.Collections.Generic;

[Serializable]
public class ResultRespone : BaseWebsocketRespone
{
    public ResultResponeData data { get; set; }
}

[Serializable]
public class ResultResponeData
{
    public List<WinArray> winArray { get; set; }
}

public class WinArray
{
    public string nickname { get; set; }
    public int point { get; set; }
    public List<CardValue> cards { get; set; }
}
