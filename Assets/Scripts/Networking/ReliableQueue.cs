using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableQueue
{
    private List<ReliableMessage> MessageQueue;
    private Dictionary<ReliableMessage, long> SentFrames;
    private Dictionary<int, ReliableMessage> gamemessages;
    private float time;

    public ReliableQueue()
    {
        MessageQueue = new List<ReliableMessage>();
        SentFrames = new Dictionary<ReliableMessage, long>();
        gamemessages = new Dictionary<int, ReliableMessage>();
    }

    public void ReceivedACK(int ackid)
    {
        Debug.Log("Received ack id: " + ackid);
        while (MessageQueue.Count > 0 && MessageQueue[0]._MessageId <= ackid)
        {
            MessageQueue.RemoveAt(0);
            SentFrames.Remove(gamemessages[ackid]);
            gamemessages.Remove(ackid);
        }
    }

    public void AddQueue(ReliableMessage gm, long frameNumber)
    {
        MessageQueue.Add(gm);
        if (SentFrames.ContainsKey(gm))
        {
            SentFrames[gm] = frameNumber;
            gamemessages[gm._MessageId] = gm;
        }
        else
        {
            SentFrames.Add(gm, frameNumber);
            gamemessages.Add(gm._MessageId, gm);
        }
    }


    public List<GameMessage> MessageToResend(long frameNumber)
    {
        List<GameMessage> needResend = new List<GameMessage>();
        foreach (ReliableMessage rm in MessageQueue)
        {
            if (frameNumber - SentFrames[rm] >= GlobalSettings.ReliableTimeout)
            {
                Debug.Log("NEED ACK OF MESSAGE " + rm._MessageId);
                needResend.Add(rm);
                SentFrames[rm] = frameNumber;
            }
        }
        
        return needResend;
    }
    
}