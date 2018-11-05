using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMessage
{
    public abstract MessageType type();
    public abstract bool isReliable();
    public abstract byte[] Serialize();
}
