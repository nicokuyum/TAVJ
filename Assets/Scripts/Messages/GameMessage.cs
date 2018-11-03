using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMessage
{
    private static int idCounter = 0;
    
    public readonly int _MessageId;
    public abstract MessageType type();
    public abstract bool isReliable();
    public abstract byte[] Serialize();

    protected GameMessage()
    {
        this._MessageId = idCounter++;
    }
}
