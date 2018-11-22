using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMessage : ReliableMessage
{
    public int targetId;
    
    public ShotMessage(int targetId, float timeStamp, bool increase) : base(timeStamp, increase)
    {
        this.targetId = targetId;
    }
    
    public ShotMessage(int messageId, int targetId, float timeStamp, bool increase) : base(timeStamp, increase)
    {
        this._MessageId = messageId;
        this.targetId = targetId;
    }

    public override MessageType type()
    {
        return MessageType.Shot;
    }

    public override byte[] Serialize()
    {
        Compressor c = new Compressor();
        c.WriteNumber((int)MessageType.Shot, Enum.GetNames(typeof(MessageType)).Length);
        c.WriteNumber(_MessageId, GlobalSettings.MaxACK);
        CompressingUtils.WriteTime(c, _TimeStamp);
        c.WriteNumber(targetId, GlobalSettings.MaxPlayers);
        return c.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor c)
    {
        c.WriteNumber((int)MessageType.Shot, Enum.GetNames(typeof(MessageType)).Length);
        c.WriteNumber(_MessageId, GlobalSettings.MaxACK);
        CompressingUtils.WriteTime(c, _TimeStamp);
        c.WriteNumber(targetId,GlobalSettings.MaxPlayers); 
    }
}
