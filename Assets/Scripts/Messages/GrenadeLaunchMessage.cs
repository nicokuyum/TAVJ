using System;
using UnityEngine;

public class GrenadeLaunchMessage : ReliableMessage
{
    private Vector3 position;
    private Vector3 direction;
    
    public GrenadeLaunchMessage(Vector3 position, Vector3 direction, float timeStamp, bool increase) : base(timeStamp, increase)
    {
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
        //compressor.WriteString(name);
        return compressor.GetBuffer();
    }

    public override void SerializeWithCompressor(Compressor c)
    {
        throw new System.NotImplementedException();
    }
}