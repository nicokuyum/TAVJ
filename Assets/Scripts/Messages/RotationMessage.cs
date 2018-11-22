using System;
using UnityEngine;

public class RotationMessage : GameMessage
{
    public Vector3 rot;
    
    public RotationMessage(Vector3 rot)
    {
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
        CompressingUtils.WriteRotation(c, rot);
        return c.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor c)
    {
        c.WriteNumber((int)MessageType.Rotation,Enum.GetNames(typeof(MessageType)).Length);
        CompressingUtils.WriteRotation(c, rot);
    }
}
