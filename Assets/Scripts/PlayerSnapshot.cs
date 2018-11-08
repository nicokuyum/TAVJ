﻿using System.Threading;
using UnityEngine;
using Random = System.Random;

public class PlayerSnapshot
{

    public int id;
    public float _TimeStamp;
    public int Health;
    public bool Invulnerable;
    public Vector3 position;
    public Quaternion rotation;
    public Player player;

    public PlayerSnapshot(int id, Vector3 position)
    {
        this.id = id;
        Health = GlobalSettings.MaxHealth;
        Invulnerable = false;
        this.position = position;
    }

    public PlayerSnapshot(int id)
    {
        this.id = id;
        Health = GlobalSettings.MaxHealth;
        Invulnerable = false;
        position = new Vector3(50, 50, 50);
    }

    public PlayerSnapshot(int id, Player player, float time)
    {
        this.id = id;
        this._TimeStamp = time;
        Health = GlobalSettings.MaxHealth;
        Random random = new Random();
        Invulnerable = false;
        int range = (int)(GlobalSettings.MaxPosition - GlobalSettings.MinPosition);
        position = new Vector3(random.Next(range) + GlobalSettings.MinPosition
            ,1, random.Next(range) + GlobalSettings.MinPosition);
        this.player = player;
        player.transform.position = position;
    }

    public byte[] serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber(id, GlobalSettings.MaxPlayers);
        CompressingUtils.WriteTime(compressor,_TimeStamp);
        compressor.WriteNumber(this.Health, GlobalSettings.MaxHealth);
        compressor.PutBit(this.Invulnerable);
        CompressingUtils.WritePosition(compressor, position);
        return compressor.GetBuffer();
    }

    public void apply(InputKey key)
    {
        throw new System.NotImplementedException();
    }

    public PlayerSnapshot RandomPositionSnapshot(int id)
    {
        Random random = new Random();
        int range = (int)(GlobalSettings.MaxPosition - GlobalSettings.MinPosition);
        Vector3 position = new Vector3(random.Next(range) + GlobalSettings.MinPosition
            ,0,  random.Next(range) + GlobalSettings.MinPosition);
        return new PlayerSnapshot(id, position);
    }
}
