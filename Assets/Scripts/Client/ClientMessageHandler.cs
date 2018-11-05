using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler
{

	private readonly ReliableQueue rq;

	private readonly List<PlayerSnapshotMessage> snapshots;


	public ClientMessageHandler(ReliableQueue rq)
	{
		this.rq = rq;
	}

	public void Handle(GameMessage gm)
	{
		switch (gm.type())
		{
			case MessageType.Ack:
				handleAck((AckMessage) gm);
				break;
			case MessageType.PlayerSnapshot:
				handlePlayerSnapshot((PlayerSnapshotMessage) gm);
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

	private void handlePlayerSnapshot(PlayerSnapshotMessage psm)
	{
		Player p = GameObject.Find("Player").GetComponent<Player>();
		p.Health = psm.Snapshot.Health;
		p.Invulnerable = psm.Snapshot.Invulnerable;
		p.gameObject.transform.position = psm.Snapshot.position;
	}

	private void handlePlayerSnapshotInterpolating(PlayerSnapshotMessage psm)
	{
		
	}

}
