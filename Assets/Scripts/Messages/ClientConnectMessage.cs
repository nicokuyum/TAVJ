using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectMessage : ReliableMessage
{
    public String name;

    public ClientConnectMessage(int _MessageId, String name)
    {
        this._MessageId = _MessageId;
        this.name = name;
    }
    
    public ClientConnectMessage(String name)
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
        compressor.WriteString(name);
        return compressor.GetBuffer();
    }
    
}
