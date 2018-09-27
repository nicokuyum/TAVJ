using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Packet
{
    private const int MaxMessagesNum = 128;
    private readonly int MessageCount;
    public String remoteIp;
    public List<GameMessage> Messages;

    public Packet(byte[] data, String remoteIp)
    {
        this.remoteIp = remoteIp;
        Decompressor decompressor = new Decompressor(data);
        MessageCount = decompressor.GetNumber(MaxMessagesNum);
        for (int i = 0; i < MessageCount; i++)
        {
            Messages.Add(MessageDeserializer.deserialize(decompressor));
        }
    }
}
