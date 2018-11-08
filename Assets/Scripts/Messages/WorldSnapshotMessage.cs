﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSnapshotMessage : GameMessage
{

    public float time;
    public int _numberOfPlayers;
    public List<PlayerSnapshot> _playerSnapshots;

    public WorldSnapshotMessage(List<PlayerSnapshot> playerSnapshots, float time)
    {
        this._playerSnapshots = new List<PlayerSnapshot>();
        foreach (PlayerSnapshot playerSnapshot in playerSnapshots)
        {
            Debug.Log("Sending snap for id " + playerSnapshot.id);
            _playerSnapshots.Add(playerSnapshot);
        }

        this._numberOfPlayers = _playerSnapshots.Count;
        this.time = time;
    }
    
    public override MessageType type()
    {
        return MessageType.WorldSnapshot;
    }

    public override bool isReliable()
    {
        return false;
    }

    public override byte[] Serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber((int)MessageType.WorldSnapshot,Enum.GetNames(typeof(MessageType)).Length);
        CompressingUtils.WriteTime(compressor, time);
        compressor.WriteNumber(_numberOfPlayers, GlobalSettings.MaxPlayers);
        foreach (PlayerSnapshot playerSnapshot in _playerSnapshots)
        {
            compressor.WriteData(playerSnapshot.serialize());
        }
        return compressor.GetBuffer();
    }
}
