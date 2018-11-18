using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerMessageHandler {

    public ServerMessageHandler()
    {
        
    }


    public void Handle(GameMessage gm)
    {
        switch (gm.type())
        {
            case MessageType.Ack:
                break;
            case MessageType.ClientConnect:
                break;
            case MessageType.PlayerInput:
                break;
        }
    }
}
