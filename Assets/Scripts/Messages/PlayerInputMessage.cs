using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMessage : GameMessage
{

    public PlayerAction Action;

    public override MessageType type()
    {
        return MessageType.PlayerInput;
    }

    public override bool isReliable()
    {
        return true;
    }
}
