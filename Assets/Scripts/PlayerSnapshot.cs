using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshot
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
}
