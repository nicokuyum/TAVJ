using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientConnectedMessage : GameMessage
{


    public int id;
    public String name;

    public ClientConnectedMessage(int id, String name)
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
        compressor.WriteString(name);
        return compressor.GetBuffer();

    }
}
