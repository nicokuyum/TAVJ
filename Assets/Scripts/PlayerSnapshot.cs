using System.Threading;
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
    public ServerPlayer player;
    public int lastId;

    public PlayerSnapshot(int id)
    {
        this.id = id;
        Health = GlobalSettings.MaxHealth;
        Invulnerable = false;
        position = new Vector3(50, 50, 50);
    }

    public PlayerSnapshot(int id, ServerPlayer player, float time)
    {
        this.id = id;
        this._TimeStamp = time;
        Health = GlobalSettings.MaxHealth;
        Random random = new Random();
        Invulnerable = false;
        int range = (int)(GlobalSettings.MaxPosition - GlobalSettings.MinPosition);
        position = new Vector3(random.Next(range) + GlobalSettings.MinPosition
            ,2, random.Next(range) + GlobalSettings.MinPosition);
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
        compressor.WriteNumber(lastId, GlobalSettings.MaxACK);
        return compressor.GetBuffer();
    }

    public void serializeWithCompressor(Compressor c)
    {
        c.WriteNumber(id, GlobalSettings.MaxPlayers);
        CompressingUtils.WriteTime(c,_TimeStamp);
        c.WriteNumber(this.Health, GlobalSettings.MaxHealth);
        c.PutBit(this.Invulnerable);
        CompressingUtils.WritePosition(c, position);
        c.WriteNumber(lastId, GlobalSettings.MaxACK);
    }

}
