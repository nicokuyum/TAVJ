using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMessage : GameMessage
{

    public PlayerAction Action;

    public PlayerInputMessage(PlayerAction action)
    {
        this.Action = action;
    }

    public override MessageType type()
    {
        return MessageType.PlayerInput;
    }

    public override bool isReliable()
    {
        return true;
    }

    public override byte[] Serialize()
    {
        throw new System.NotImplementedException();
    }
}
