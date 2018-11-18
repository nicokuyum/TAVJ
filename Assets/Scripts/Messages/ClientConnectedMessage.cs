using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ClientConnectedMessage : ReliableMessage
{

    public int id;
    public String name;

    public ClientConnectedMessage(int id, String name, float timeStamp, bool increaseCounter) : base(timeStamp,
        increaseCounter)
    {
        this.id = id;
        this.name = name;
    }

    public ClientConnectedMessage(int _MessageId, int id, String name, float timeStamp, bool increaseCounter): base(timeStamp, increaseCounter)
    {
        this._MessageId = _MessageId;
        this.id = id;
        this.name = name;
    }
    
    

    public override MessageType type()
    {
        return MessageType.ConnectConfirmation;
    }

    public override bool isReliable()
    {
        return true;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.ConnectConfirmation, Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, GlobalSettings.MaxACK);
        compressor.WriteNumber(id, GlobalSettings.MaxPlayers);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        compressor.WriteString(name);
        return compressor.GetBuffer();
    }
}
