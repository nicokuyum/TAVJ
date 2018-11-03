using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class MessageSerializer {

	public static GameMessage deserialize(Decompressor decompressor)
	{
		MessageType type = (MessageType) decompressor.GetNumber(Enum.GetNames(typeof(MessageType)).Length);
		switch (type)
		{
				case MessageType.Ack:
					return AckDeserialize(decompressor);
				case MessageType.ClientConnect:
					return ClientConnectDeserialiaze(decompressor);
				case MessageType.PlayerInput:		// TODO
					return new PlayerInputMessage();
				case MessageType.PlayerSnapshot:
					return PlayerSnapshotDeserialize(decompressor);
					break;
				default: return null;
		}

		return null;
	}

	public static GameMessage AckDeserialize(Decompressor decompressor)
	{
		return new AckMessage(decompressor.GetNumber(GlobalSettings.MaxACK));
	}

	public static GameMessage ClientConnectDeserialiaze(Decompressor decompressor)
	{
		return new ClientConnectMessage(decompressor.GetString());
	}

	public static GameMessage PlayerSnapshotDeserialize(Decompressor decompressor)
	{
		Vector3 position = new Vector3();
		PlayerSnapshot playerSnapshot = new PlayerSnapshot();
		playerSnapshot.frameNumber = decompressor.GetNumber(3600 * (long) GlobalSettings.Fps);
		playerSnapshot.Health = decompressor.GetNumber(GlobalSettings.MaxHealth);
		playerSnapshot.Invulnerable = decompressor.GetBoolean();
		position.x = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
		position.y = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
		position.z = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
		return new PlayerSnapshotMessage(playerSnapshot);
	}
	



}
