using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMessage
{
    public int _MessageId;
    public abstract MessageType type();
    public abstract bool isReliable();
}
