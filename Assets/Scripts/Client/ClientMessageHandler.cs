using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler
{

	private readonly ReliableQueue rq;
	private List<GameMessage> outgoingGameMessages;
	private Player player;
	private Dictionary<int, Player> otherPlayers;
	private GameObject playerPrefab;

	public ClientMessageHandler(ReliableQueue rq, Player player, Dictionary<int, Player> otherPlayers,
		List<GameMessage> outgoingGameMessages, GameObject playerPrefab)
	{
		this.player = player;
		this.otherPlayers = otherPlayers;
		this.rq = rq;
		this.outgoingGameMessages = outgoingGameMessages;
		this.playerPrefab = playerPrefab;
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
		//Debug.Log("Handling ack with ackid " + message.ackid);
		rq.ReceivedACK(message.ackid);
	}

	private void handlePlayerSnapshotInterpolating(PlayerSnapshotMessage psm)
	{
		SnapshotHandler.GetInstance().ReceiveSnapshot(psm.Snapshot);
		/*player.Health = psm.Snapshot.Health;
		player.Invulnerable = psm.Snapshot.Invulnerable;
		player.gameObject.transform.position = psm.Snapshot.position;
		player.gameObject.transform.rotation = psm.Snapshot.rotation;*/
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
				GameObject go = GameObject.Instantiate(playerPrefab);
				Player newPlayer = go.GetComponent<Player>();
				newPlayer.id = ccm.id;
				newPlayer.name = ccm.name;
				otherPlayers.Add(ccm.id, newPlayer);
			}
			// Else ignore
		}
		Debug.Log("Adding Ack for message id " + ccm._MessageId);
		outgoingGameMessages.Add(new AckMessage(ccm._MessageId));
	}

}
