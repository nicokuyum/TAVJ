using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReliableQueue
{
    //Messages with pending ACK
    private List<ReliableMessage> MessageQueue;
    
    //Time at which message was last resent
    private Dictionary<ReliableMessage, float> SentFrames;
    
    //ACKID to message mapping
    private Dictionary<int, ReliableMessage> gamemessages;

    public ReliableQueue()
    {
        MessageQueue = new List<ReliableMessage>();
        SentFrames = new Dictionary<ReliableMessage, float>();
        gamemessages = new Dictionary<int, ReliableMessage>();
    }

    public void ReceivedACK(int ackid)
    {
        while (MessageQueue.Count > 0 && MessageQueue[0]._MessageId <= ackid)
        {
            ReliableMessage toRemove = MessageQueue[0];
            MessageQueue.RemoveAt(0);
            SentFrames.Remove(gamemessages[toRemove._MessageId]);
            gamemessages.Remove(toRemove._MessageId);
        }
    }

    
    public void AddQueue(ReliableMessage gm, float time)
    {
        MessageQueue.Add(gm);
        if (SentFrames.ContainsKey(gm))
        {
            SentFrames[gm] = 0;
            gamemessages[gm._MessageId] = gm;
        }
        else
        {
            SentFrames.Add(gm, 0);
            gamemessages.Add(gm._MessageId, gm);
        }
    }


    public List<GameMessage> MessageToResend(float time)
    {
        List<GameMessage> needResend = new List<GameMessage>();
        foreach (ReliableMessage rm in MessageQueue)
        {
            if (time - SentFrames[rm] >= GlobalSettings.ReliableTimeout)
            {
                //Debug.Log("NEED ACK OF MESSAGE " + rm._MessageId);
                needResend.Add(rm);
                SentFrames[rm] = time;
            }
        }
        return needResend;
    }
    
}