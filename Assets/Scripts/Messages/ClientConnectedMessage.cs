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

    public ClientConnectedMessage(int id, String name, float timeStamp): base(timeStamp)
    {
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
        compressor.WriteNumber(id, GlobalSettings.MaxPlayers);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        compressor.WriteString(name);
        return compressor.GetBuffer();
    }
}
