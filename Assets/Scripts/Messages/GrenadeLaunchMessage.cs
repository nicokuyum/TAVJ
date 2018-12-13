using System;
using UnityEngine;

public class GrenadeLaunchMessage : ReliableMessage
{
    public Vector3 position;
    public Vector3 direction;
    
    public GrenadeLaunchMessage(Vector3 position, Vector3 direction, float timeStamp, bool increase) : base(timeStamp, increase)
    {
        this.position = position;
        this.direction = direction;
    }
    
    public GrenadeLaunchMessage(int id, Vector3 position, Vector3 direction, float timeStamp, bool increase) : base(timeStamp, increase)
    {
        this._MessageId = id;
        this.position = position;
        this.direction = direction;
    }

    public override MessageType type()
    {
        return MessageType.Grenade;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.ConnectConfirmation, Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, GlobalSettings.MaxACK);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        CompressingUtils.WritePosition(compressor, position);
        CompressingUtils.WritePosition(compressor, direction);
        return compressor.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor compressor)
    {
        compressor.WriteNumber((int)MessageType.ConnectConfirmation, Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, GlobalSettings.MaxACK);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        CompressingUtils.WritePosition(compressor, position);
        CompressingUtils.WritePosition(compressor, direction);
    }
}