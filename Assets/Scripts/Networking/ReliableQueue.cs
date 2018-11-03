using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableQueue
{
    private List<GameMessage> MessageQueue;
    private Dictionary<GameMessage, long> SentFrames;

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
        foreach(KeyValuePair<GameMessage, long> entry in SentFrames)
        {
            
        }

        foreach (GameMessage gm in SentFrames.Keys)
        {
            SentFrames[gm] += 1;
       
        }

        return null;
    }
}
