using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableQueue
{
    private List<GameMessage> MessageQueue;
    private Dictionary<GameMessage, long> SentFrames;
    private Dictionary<int, GameMessage> gamemessages;

    public ReliableQueue()
    {
        MessageQueue = new List<GameMessage>();
        SentFrames = new Dictionary<GameMessage, long>();
        gamemessages = new Dictionary<int, GameMessage>();
        
    }

    public void ReceivedACK(int ackid)
    {
        while (MessageQueue[0]._MessageId <= ackid)
        {
            MessageQueue.RemoveAt(0);
            SentFrames.Remove(gamemessages[ackid]);
            gamemessages.Remove(ackid);
        }
    }

    public void AddQueue(GameMessage gm, long frameNumber)
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

        foreach (GameMessage gm in MessageQueue)
        {
            if (frameNumber - SentFrames[gm] >= GlobalSettings.ReliableTimeout)
            {
                needResend.Add(gm);
                SentFrames[gm] = frameNumber;
            }
        }
        return needResend;
    }
    
}