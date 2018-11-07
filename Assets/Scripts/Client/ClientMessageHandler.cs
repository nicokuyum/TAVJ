using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler
{

	private readonly ReliableQueue rq;
	private List<GameMessage> outgoingGameMessages;
	private Player player;
	private Dictionary<int, Player> otherPlayers;

	public ClientMessageHandler(ReliableQueue rq, Player player, Dictionary<int, Player> otherPlayers, List<GameMessage> outgoingGameMessages)
	{
		this.player = player;
		this.otherPlayers = otherPlayers;
		this.rq = rq;
		this.outgoingGameMessages = outgoingGameMessages;
	}

	public void Handle(GameMessage gm)
	{
		switch (gm.type())
		{
			case MessageType.Ack:
				handleAck((AckMessage) gm);
				break;
			case MessageType.PlayerSnapshot:
				handlePlayerSnapshotInterpolating((PlayerSnapshotMessage) gm);
				break;
			case MessageType.ConnectConfirmation:
				handleConnectionConfirmation((ClientConnectedMessage) gm);
				break;
			default:
				throw new NotImplementedException();
		}
	}

	private void handleAck(AckMessage message)
	{
		Debug.Log("Handling ack with ackid " + message.ackid);
		rq.ReceivedACK(message.ackid);
	}

	private void handlePlayerSnapshotInterpolating(PlayerSnapshotMessage psm)
	{
		SnapshotHandler.GetInstance().ReceiveSnapshot(psm.Snapshot);
	}

	private void handleConnectionConfirmation(ClientConnectedMessage ccm)
	{
		if (player.name.Equals(ccm.name))
		{
			this.player.id = ccm.id;
		}
		else
		{
			if (!otherPlayers.ContainsKey(ccm.id))
			{
				Player newPlayer = new Player();
				newPlayer.id = ccm.id;
				newPlayer.name = ccm.name;
				otherPlayers.Add(ccm.id, newPlayer);
			}
			// Else ignore
		}
		
		//outgoingGameMessages.Add(new AckMessage(ccm.));
	}

}
