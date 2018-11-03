using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Networking;

public class Packet
{
    private const int MaxMessagesNum = 128;
    public readonly int MessageCount;
    public Connection connection;
    public List<GameMessage> Messages;

    public Packet(byte[] data, Connection connection)
    {
        Messages = new List<GameMessage>();
        this.connection = connection;
        Decompressor decompressor = new Decompressor(data);
        MessageCount = decompressor.GetNumber(MaxMessagesNum);
        for (int i = 0; i < MessageCount; i++)
        {
           // Messages.Add(MessageSerializer.deserialize(decompressor));
        }
    }

    public Packet(List<GameMessage> messages)
    {
        this.MessageCount = messages.Count;
        this.Messages = messages;
    }

    public byte[] serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber(MessageCount, MaxMessagesNum);
        foreach (var gm in Messages)
        {
            //compressor.WriteData(gm.Serialize());
        }

        return compressor.GetBuffer();
    }
}
