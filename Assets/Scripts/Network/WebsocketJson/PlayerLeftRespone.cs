

using System;
using System.Collections.Generic;

[Serializable]
public class PlayerLeftRespone : BaseWebsocketRespone
{
    public PlayerLeftResponeData data { get; set; }
}

[Serializable]
public class PlayerLeftResponeData
{
    public string nickname { get; set; }
    public string userType { get; set; }
    public string newMatser { get; set; }
    public List<PlayerPosition> position { get; set; }
}
