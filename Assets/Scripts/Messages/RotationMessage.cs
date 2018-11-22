using System;
using UnityEngine;

public class RotationMessage : GameMessage
{
    public int playerId;
    public Vector3 rot;
    
    public RotationMessage(int id, Vector3 rot)
    {
        this.playerId = id;
        this.rot = rot;
    }
    
    public override MessageType type()
    {
        return MessageType.Rotation;
    }

    public override bool isReliable()
    {
        return false;
    }

    public override byte[] Serialize()
    {
        Compressor c = new Compressor();
        c.WriteNumber((int)MessageType.Rotation,Enum.GetNames(typeof(MessageType)).Length);
        c.WriteNumber(playerId, GlobalSettings.MaxPlayers);
        CompressingUtils.WritePosition(c, rot);
        return c.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor c)
    {
        c.WriteNumber((int)MessageType.Rotation,Enum.GetNames(typeof(MessageType)).Length);
        c.WriteNumber(playerId, GlobalSettings.MaxPlayers);
        CompressingUtils.WritePosition(c, rot);
    }
}
