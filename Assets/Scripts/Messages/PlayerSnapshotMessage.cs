using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshotMessage : GameMessage {
    
    public override MessageType type()
    {
        return MessageType.PlayerSnapshot;
    }

    public override bool isReliable()
    {
        return false;
    }
}
