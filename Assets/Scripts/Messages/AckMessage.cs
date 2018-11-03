using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AckMessage : GameMessage
{

    public int ackid;
    
    public override MessageType type()
    {
        return MessageType.Ack;
    }

    public override bool isReliable()
    {
        return false;
    }
}
