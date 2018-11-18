using System;

public class PlayerInputMessage : ReliableMessage
{

    public PlayerAction Action;

    public PlayerInputMessage(PlayerAction action, float timeStamp, bool increaseCoumter): base(timeStamp, increaseCoumter)
    {
        this.Action = action;
    }

    public PlayerInputMessage(PlayerAction action, int id, float timeStamp, bool increaseCounter): base(timeStamp, increaseCounter)
    {
        _MessageId = id;
        Action = action;
    }

    public override MessageType type()
    {
        return MessageType.PlayerInput;
    }

    public override bool isReliable()
    {
        return true;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.PlayerInput,Enum.GetNames(typeof(MessageType)).Length);
        compressor.WriteNumber(_MessageId, int.MaxValue);
        CompressingUtils.WriteTime(compressor, _TimeStamp);
        compressor.WriteNumber((int)Action, Enum.GetNames(typeof(PlayerAction)).Length);
        return compressor.GetBuffer();
    }
}
