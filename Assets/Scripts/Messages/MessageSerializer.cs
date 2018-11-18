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
				default: return null;
		}
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
		return new ClientConnectMessage(decompressor.GetNumber(int.MaxValue),name, time, false);
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
		Debug.Log("Received last ID " + playerSnapshot.lastId);
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
		int id = decompressor.GetNumber(GlobalSettings.MaxPlayers);
		float timeStamp = CompressingUtils.GetTime(decompressor);
		String name = decompressor.GetString();
		return new ClientConnectedMessage(id, name, timeStamp, false);
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
		Debug.Log("Received last ID " + playerSnapshot.lastId);
		return playerSnapshot;
	}

}
