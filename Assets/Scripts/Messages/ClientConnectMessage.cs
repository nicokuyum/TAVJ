using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectMessage : ReliableMessage
{
    public String name;

    public ClientConnectMessage(int _MessageId, String name, float timeStamp): base(timeStamp)
    {
        this._MessageId = _MessageId;
        this.name = name;
    }
    
    public ClientConnectMessage(String name, float timeStamp): base(timeStamp)
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
    
}
