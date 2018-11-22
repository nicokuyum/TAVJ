using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler
{
	private Client client;

	public ClientMessageHandler(Client client)
	{
		this.client = client;
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
			case MessageType.WorldSnapshot:
				handleWorldSnapshot((WorldSnapshotMessage) gm);
				break;
			default:
				throw new NotImplementedException();
		}
	}

	private void handleAck(AckMessage message)
	{
		client.GetReliableQueue().ReceivedACK(message.ackid);
	}

	private void handlePlayerSnapshotInterpolating(PlayerSnapshotMessage psm)
	{
		throw new NotImplementedException();
		//SnapshotHandler.GetInstance().ReceiveSnapshot(psm.Snapshot);
	}

	private void handleConnectionConfirmation(ClientConnectedMessage ccm)
	{
		Debug.Log("Received name: " + ccm.name + " with id " + ccm.id);
		if (client.getPlayer().name.Equals(ccm.name))
		{
			client.setTime(ccm._TimeStamp);
			this.client.getPlayer().id = ccm.id;
		}
		else
		{
			if (!client.getOtherPlayers().ContainsKey(ccm.id))
			{
				GameObject go = GameObject.Instantiate(client.otherPlayersPrefab);
				ServerPlayer newPlayer = go.GetComponent<ServerPlayer>();
				newPlayer.id = ccm.id;
				newPlayer.name = ccm.name;
				client.getOtherPlayers().Add(ccm.id, newPlayer);
			}
			// Else ignore
		}
		client.getOutgoingMessages().Add(new AckMessage(ccm._MessageId));
	}

	private void Instantiate(GameObject getPlayerPrefab)
	{
		throw new NotImplementedException();
	}

	private void handleWorldSnapshot(WorldSnapshotMessage wm)
	{
		float time = wm._playerSnapshots[0]._TimeStamp;
		Dictionary<int, PlayerSnapshot> worldSnap = new Dictionary<int, PlayerSnapshot>();
		
		
		foreach (PlayerSnapshot wmPlayerSnapshot in wm._playerSnapshots)
		{
			worldSnap.Add(wmPlayerSnapshot.id, wmPlayerSnapshot);
		}
		
		SnapshotHandler.GetInstance().ReceiveSnapshot(worldSnap, time);
	}
}
