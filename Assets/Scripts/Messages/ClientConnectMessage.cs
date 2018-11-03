using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectMessage : GameMessage {
    public override MessageType type()
    {
        return MessageType.ClientConnect;
    }

    public override bool isReliable()
    {
        return true;
    }
}
