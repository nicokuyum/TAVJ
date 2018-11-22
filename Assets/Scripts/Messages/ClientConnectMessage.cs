using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectMessage : ReliableMessage
{
    public String name;

    public ClientConnectMessage(int _MessageId, String name, float timeStamp, bool increaseCounter): base(timeStamp, increaseCounter)
    {
        this._MessageId = _MessageId;
        this.name = name;
    }
    
    public ClientConnectMessage(String name, float timeStamp, bool increaseCounter): base(timeStamp, increaseCounter)
    {
        this.name = name;
    }
    
    public override MessageType type()
    {
        return MessageType.ClientConnect;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.ClientConnect,Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, int.MaxValue);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        compressor.WriteString(name);
        return compressor.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor c)
    {
        c.WriteNumber((int)MessageType.ClientConnect,Enum.GetNames(typeof(MessageType)).Length);
        c.WriteNumber(_MessageId, int.MaxValue);
        CompressingUtils.WriteTime(c, _TimeStamp);
        c.WriteString(name);
    }
}
