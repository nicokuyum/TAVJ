using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReliableQueue
{
    //Messages with pending ACK
    private List<ReliableMessage> MessageQueue;
    
    //Time at which message was last resent
    private Dictionary<ReliableMessage, float> SentFrames;
    
    //ACKID to message mapping
    private Dictionary<int, ReliableMessage> gamemessages;
    
    //No timeout messages
    private List<ReliableMessage> NoTimeOutQueue;

    
    private float timeout;

    public ReliableQueue(float timeout)
    {
        this.timeout = timeout;
        MessageQueue = new List<ReliableMessage>();
        NoTimeOutQueue = new List<ReliableMessage>();
        SentFrames = new Dictionary<ReliableMessage, float>();
        gamemessages = new Dictionary<int, ReliableMessage>();
    }

    public void ReceivedACK(int ackid)
    {
        var a = 5;
        while (MessageQueue.Count > 0 && MessageQueue[0]._MessageId <= ackid)
        {
            ReliableMessage toRemove = MessageQueue[0];
            MessageQueue.RemoveAt(0);
            SentFrames.Remove(gamemessages[toRemove._MessageId]);
            gamemessages.Remove(toRemove._MessageId);
        }

        while (NoTimeOutQueue.Count() > 0 && NoTimeOutQueue[0]._MessageId <= ackid)
        {
            Debug.Log("Removing from NoTimeoutQueue ackid " + ackid);
            NoTimeOutQueue.RemoveAt(0);
        }
    }

    
    public void AddQueueWithTimeout(ReliableMessage gm, float time)
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

    public void AddQueueWithOutTimeout(ReliableMessage gm)
    {
        NoTimeOutQueue.Add(gm);
    }


    public List<GameMessage> MessageToResend(float time)
    {
        List<GameMessage> needResend = new List<GameMessage>();
        foreach (ReliableMessage rm in MessageQueue)
        {
            if (time - SentFrames[rm] >= timeout)
            {
                needResend.Add(rm);
                SentFrames[rm] = time;
                //Debug.Log(rm.type().ToString());
                Debug.Log("Need ACK: " + rm._MessageId);
            }
        }

        foreach (ReliableMessage rm in NoTimeOutQueue)
        {
            Debug.Log(rm.type().ToString());
            needResend.Add(rm);
        }
        return needResend;
    }

    public int getCount()
    {
        return MessageQueue.Count + NoTimeOutQueue.Count;
    }
}