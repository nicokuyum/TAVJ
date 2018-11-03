using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshotMessage : GameMessage
{
    private PlayerSnapshot snapshot;

    
    public PlayerSnapshotMessage(PlayerSnapshot snapshot)
    {
        this.snapshot = snapshot;
    }

    public override MessageType type()
    {
        return MessageType.PlayerSnapshot;
    }

    public override bool isReliable()
    {
        return false;
    }

    public byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.PlayerSnapshot,Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteData(snapshot.serialize());
        return compressor.GetBuffer();
    }


}
