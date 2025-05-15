using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatContentModel : BaseWebsocketRequest
{
 public ChatContentModelData data { get; set; }
 

    public ChatContentModel(int eventType, ChatContentModelData data)
    {
        this.eventType = eventType;
        this.data = data;
    }
}
[Serializable]
public class ChatContentModelData
{
    public ChatContentModelData2 data { get; set; }


}
[Serializable]
public class ChatContentModelData2
{
    public string chatContent { get; set; }
    public string nickname { get; set; }


}

//

[Serializable]
public class ChatContentRespone : BaseWebsocketRespone
{
    public ChatContentModelData2 data { get; set; }
}



