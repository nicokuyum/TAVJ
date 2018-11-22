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
					return ClientConnectDeserialize(decompressor);
				case MessageType.PlayerInput:
					return PlayerInputDeserialize(decompressor);
				case MessageType.PlayerSnapshot:
					return PlayerSnapshotDeserialize(decompressor);
				case MessageType.ConnectConfirmation:
					return ConnectedClientDeserialize(decompressor);
				case MessageType.WorldSnapshot:
					return WorldSnapshotDeserialize(decompressor);
				case MessageType.Shot:
					return ShotDeserialize(decompressor);
				case MessageType.Rotation:
					return RotationDeserialize(decompressor);
				default: return null;
		}
	}

	private static GameMessage ShotDeserialize(Decompressor decompressor)
	{
		int id = decompressor.GetNumber(GlobalSettings.MaxACK);
		float time = CompressingUtils.GetTime(decompressor);
		int targetid = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		return new ShotMessage(id, targetid, time, false);
	}

	public static GameMessage AckDeserialize(Decompressor decompressor)
	{
		return new AckMessage(decompressor.GetNumber(GlobalSettings.MaxACK));
	}

	public static GameMessage ClientConnectDeserialize(Decompressor decompressor)
	{
		int id = decompressor.GetNumber(int.MaxValue);
		float time = CompressingUtils.GetTime(decompressor);
		String name = decompressor.GetString();
		return new ClientConnectMessage(id, name, time, false);
	}

	
	public static GameMessage PlayerSnapshotDeserialize(Decompressor decompressor)
	{
		int id = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		PlayerSnapshot playerSnapshot = new PlayerSnapshot(id);
		playerSnapshot._TimeStamp = CompressingUtils.GetTime(decompressor);
		playerSnapshot.Health = decompressor.GetNumber(GlobalSettings.MaxHealth);
		playerSnapshot.Invulnerable = decompressor.GetBoolean();
		playerSnapshot.position = CompressingUtils.GetPosition(decompressor);
		
		playerSnapshot.lastId = decompressor.GetNumber(GlobalSettings.MaxACK);
		return new PlayerSnapshotMessage(playerSnapshot);
	}

	public static GameMessage PlayerInputDeserialize(Decompressor decompressor)
	{
		int id = decompressor.GetNumber(int.MaxValue);
		float time = CompressingUtils.GetTime(decompressor);
		PlayerAction action = (PlayerAction)decompressor.GetNumber(Enum.GetNames(typeof(PlayerAction)).Length);	
		return new PlayerInputMessage(action, id, time, false);
	}


	public static GameMessage ConnectedClientDeserialize(Decompressor decompressor)
	{
		int mssgid = decompressor.GetNumber(GlobalSettings.MaxACK);
		int id = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		float timeStamp = CompressingUtils.GetTime(decompressor);
		String name = decompressor.GetString();
		return new ClientConnectedMessage(mssgid, id, name, timeStamp, false);
	}

	public static GameMessage WorldSnapshotDeserialize(Decompressor decompressor)
	{
		float time = CompressingUtils.GetTime(decompressor);
		
		int numberOfPlayers = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		List<PlayerSnapshot> playerSnapshots = new List<PlayerSnapshot>();
		for (int i = 0; i < numberOfPlayers; i++)
		{
			//Should use snapshotdeserialize
			playerSnapshots.Add(SnapshotDeserialize(decompressor));
		}
		return new WorldSnapshotMessage(playerSnapshots,time);
	}

	public static PlayerSnapshot SnapshotDeserialize(Decompressor decompressor)
	{
		int id = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		PlayerSnapshot playerSnapshot = new PlayerSnapshot(id);
		playerSnapshot._TimeStamp = CompressingUtils.GetTime(decompressor);
		playerSnapshot.Health = decompressor.GetNumber(GlobalSettings.MaxHealth);
		playerSnapshot.Invulnerable = decompressor.GetBoolean();
		playerSnapshot.position = CompressingUtils.GetPosition(decompressor);
		playerSnapshot.lastId = decompressor.GetNumber(GlobalSettings.MaxACK);
		return playerSnapshot;
	}

	public static GameMessage RotationDeserialize(Decompressor decompressor)
	{
		Vector3 rot = CompressingUtils.GetPosition(decompressor);
		return new RotationMessage(rot);
	}
}
