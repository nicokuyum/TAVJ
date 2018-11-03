using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectMessage : GameMessage
{
    private String name;

    public ClientConnectMessage(String name)
    {
        this.name = name;
    }
    
    public override MessageType type()
    {
        return MessageType.ClientConnect;
    }

    public override bool isReliable()
    {
        return true;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.ClientConnect,Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteString(name);
        return compressor.GetBuffer();
    }
    
}
