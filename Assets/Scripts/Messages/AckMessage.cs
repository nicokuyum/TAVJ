using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AckMessage : GameMessage
{
    public int ackid;

    public AckMessage(int _MessageId, int ackid)
    {
        this._MessageId = _MessageId;
        this.ackid = ackid;
    }

    public AckMessage(int ackid)
    {
        this.ackid = ackid;
    }

    public override MessageType type()
    {
        return MessageType.Ack;
    }

    public override bool isReliable()
    {
        return false;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.Ack, Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, int.MaxValue);
        compressor.WriteNumber(ackid, Int32.MaxValue);
        return compressor.GetBuffer();
    }
    
}
