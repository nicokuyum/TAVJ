using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshot : Serializable<PlayerSnapshot>
{

    private long frameNumber;
    private int MaxHealth;
    private int Health;
    private bool Invulnerable;
    private Vector3 position;
    private Vector3 rotation;


    public PlayerSnapshot(byte[] data)
    {
        position = new Vector3();
        Decompressor decompressor = new Decompressor(data);
        this.frameNumber = decompressor.GetNumber(GlobalSettings.MaxMatchDuration * GlobalSettings.Fps);
        this.Health = decompressor.GetNumber(GlobalSettings.MaxHealth);
        this.Invulnerable = decompressor.GetBoolean();
        position.x = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        position.y = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        position.z = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
     
    }

    public PlayerSnapshot(Vector3 position)
    {
        this.position = position;
        frameNumber = 1;
        MaxHealth = GlobalSettings.MaxHealth;
        Health = MaxHealth;
        Invulnerable = false;
        rotation = new Vector3(50,50,50);
    }

    public PlayerSnapshot()
    {
        position = new Vector3(50,50,50);
        frameNumber = 1;
        MaxHealth = GlobalSettings.MaxHealth;
        Health = MaxHealth;
        Invulnerable = false;
        rotation = new Vector3(50,50,50);
    }

    public byte[] serialize()
    {
        
        throw new System.NotImplementedException();
    }

    public void deserialize(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public void apply(InputKey key)
    {
        throw new System.NotImplementedException();
    }
}
