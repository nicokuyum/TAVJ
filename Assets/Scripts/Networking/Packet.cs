using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Networking;

public class Packet
{
    private const int MaxMessagesNum = 128;
    private readonly int MessageCount;
    public Connection connection;
    public List<GameMessage> Messages;

    public Packet(byte[] data, Connection connection)
    {
        this.connection = connection;
        Decompressor decompressor = new Decompressor(data);
        MessageCount = decompressor.GetNumber(MaxMessagesNum);
        for (int i = 0; i < MessageCount; i++)
        {
            Messages.Add(MessageSerializer.deserialize(decompressor));
        }
    }
}
