using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class MessageDeserializer {

	public static GameMessage deserialize(Decompressor decompressor)
	{
		MessageType type = (MessageType) decompressor.GetNumber(Enum.GetNames(typeof(MessageType)).Length);
		switch (type)
		{
				case MessageType.Ack: 				// TODO
					break;
				case MessageType.ClientConnect:		// TODO
					break;
				case MessageType.PlayerInput:		// TODO
					return new PlayerInputMessage();
				case MessageType.PlayerSnapshot:	// TODO
					return new PlayerSnapshotMessage();
				default: return null;
		}

		return null;
	}
}
