using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableQueue
{
    private List<GameMessage> MessageQueue;
    private Dictionary<GameMessage, long> SentFrames;

    public ReliableQueue()
    {
        MessageQueue = new List<GameMessage>();
        SentFrames = new Dictionary<GameMessage, long>();
    }

    public void ReceivedACK(int ackid)
    {
        while (MessageQueue[0]._MessageId <= ackid)
        {
            MessageQueue.RemoveAt(0);
        }
    }

    public void AddQueue(GameMessage gm, long frameNumber)
    {
        MessageQueue.Add(gm);
        SentFrames.Add(gm, frameNumber);
    }


    public List<GameMessage> MessageToResend(long frameNumber)
    {
        List<GameMessage> NeedResend = new List<GameMessage>();
        foreach(KeyValuePair<GameMessage, long> entry in SentFrames)
        {
            if (frameNumber - entry.Value >= GlobalSettings.ReliableTimeout)
            {
                NeedResend.Add(entry.Key);
            }
        }

        foreach (GameMessage gm in SentFrames.Keys)
        {
            if (frameNumber - SentFrames[gm] >= GlobalSettings.ReliableTimeout)
            {
                NeedResend.Add(gm);
                SentFrames[gm] = frameNumber;
            }
        }
        return NeedResend;
    }
    
}