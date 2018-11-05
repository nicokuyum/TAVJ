using UnityEngine;

public class PlayerSnapshot
{

    public int id;
    public long frameNumber;
    public int Health;
    public bool Invulnerable;
    public Vector3 position;
    public Quaternion rotation;
    


    public PlayerSnapshot(byte[] data)
    {
        position = new Vector3();
        Decompressor decompressor = new Decompressor(data);
        this.id = decompressor.GetNumber(GlobalSettings.MaxPlayers);
        this.frameNumber = decompressor.GetNumber(GlobalSettings.MaxMatchDuration * GlobalSettings.Fps);
        this.Health = decompressor.GetNumber(GlobalSettings.MaxHealth);
        this.Invulnerable = decompressor.GetBoolean();
        position.x = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        position.y = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
        position.z = decompressor.GetFloat(GlobalSettings.MaxPosition, GlobalSettings.MinPosition, GlobalSettings.PositionPrecision);
    }

    public PlayerSnapshot(int id, Vector3 position)
    {
        this.id = id;
        frameNumber = 1;
        Health = GlobalSettings.MaxHealth;
        Invulnerable = false;
        this.position = position;
    }

    public PlayerSnapshot(int id)
    {
        this.id = id;
        frameNumber = 1;
        Health = GlobalSettings.MaxHealth;
        Invulnerable = false;
        position = new Vector3(50, 50, 50);
    }

    public byte[] serialize()
    {
        Compressor compressor = new Compressor();
        compressor.WriteNumber(id, GlobalSettings.MaxPlayers);
        compressor.WriteNumber(frameNumber, 3600 * (long)GlobalSettings.Fps);
        compressor.WriteNumber(this.Health, GlobalSettings.MaxHealth);
        compressor.PutBit(this.Invulnerable);
        compressor.WriteFloat(position.x, GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
        compressor.WriteFloat(position.y, GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
        compressor.WriteFloat(position.z, GlobalSettings.MaxPosition, GlobalSettings.MinPosition, 0.1f);
        return compressor.GetBuffer();
    }

    public void apply(InputKey key)
    {
        throw new System.NotImplementedException();
    }
}
