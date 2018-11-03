using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshotMessage : GameMessage
{

    public PlayerSnapshot Snapshot;

    public PlayerSnapshotMessage(PlayerSnapshot ps)
    {
        this.Snapshot = ps;
    }
    
    public override MessageType type()
    {
        return MessageType.PlayerSnapshot;
    }

    public override bool isReliable()
    {
        return false;
    }
}
