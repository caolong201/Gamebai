using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePrivateTableModel : BaseWebsocketRequest
{
    public CreatePrivateTableModelData data { get; set; }

    public CreatePrivateTableModel(int eventType, CreatePrivateTableModelData data)
    {
        this.eventType = eventType;
        this.data = data;
    }

}
[Serializable]
public class CreatePrivateTableModelData
{
  

    public int betAmount { get; set; }
    public string password { get; set; }
    public int playerCount { get; set; }

}
