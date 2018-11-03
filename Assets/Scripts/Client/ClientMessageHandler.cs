using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler {
	
	ReliableQueue rq = new ReliableQueue();

	public void Handle(GameMessage gm)
	{
		switch (gm.type())
		{
			case MessageType.Ack:
				handleAck((AckMessage)gm);
				break;
			case MessageType.PlayerSnapshot:
				handlePlayerSnapshot((PlayerSnapshotMessage)gm);
				break;
			default:
				throw new NotImplementedException();
		}
	}

	private void handleAck(AckMessage message)
	{
		rq.receiveAck(message.ackid);
	}

	private void handlePlayerSnapshot(PlayerSnapshotMessage psm)
	{
		Player p = GameObject.Find("Player").GetComponent<Player>();
		psm.Snapshot.
	}
}
