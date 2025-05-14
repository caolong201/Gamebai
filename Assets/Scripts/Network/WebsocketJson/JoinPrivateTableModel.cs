using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class JoinPrivateTableModel : BaseWebsocketRequest
{
 public JoinPrivateTableModelData data { get; set; }

    public JoinPrivateTableModel(int eventType, JoinPrivateTableModelData data)
    {
        this.eventType = eventType;
        this.data = data;
    }
}

[Serializable]
public class JoinPrivateTableModelData
{
    public string username { get; set; }
    public string password { get; set; }
}

