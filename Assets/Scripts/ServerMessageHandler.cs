using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class ServerMessageHandler
{

    private Server server;
    
    public ServerMessageHandler(Server server)
    {
        this.server = server;
    }
    
    public void ProcessMessage(GameMessage gm, Connection connection)
    {
        switch (gm.type())
        {
            case MessageType.ClientConnect:
                processConnect((ClientConnectMessage)gm, connection);
                break;
            case MessageType.PlayerInput:
                processInput((PlayerInputMessage)gm, connection);
                break;
            case MessageType.Ack:
                Debug.Log("Received ACK " +  ((AckMessage)gm).ackid);
                processAck((AckMessage) gm, connection);
                break;
            case MessageType.Shot:
                Debug.Log("Received Shot on " + ((ShotMessage)gm).targetId);
                processShot((ShotMessage) gm, connection);
                break;
            default:
                throw new NotImplementedException();
                break;
        }
    }
    
    private void processInput(PlayerInputMessage inputMessage, Connection connection)
    {	
        int id = server.connections[connection];
        server.actions[id].Add(inputMessage);
    }
    
    public void processAck(AckMessage gm, Connection connection)
    {
        int id = server.connections[connection];
        server.rq[id].ReceivedACK(gm.ackid);
    }
    
    private void processConnect(ClientConnectMessage ccm, Connection connection)
    {
        if (!server.connections.ContainsKey(connection))
        {
            server.EstablishConnection(connection, ccm._MessageId, ccm.name);
        }
    }

    private void processShot(ShotMessage sm, Connection connection)
    {
        int id = server.connections[connection];
        int target = sm.targetId;
        server.players[target].Health -= 20;
    }
}
