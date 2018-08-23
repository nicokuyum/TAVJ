using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour, Serializable<Player>
{

	public int MaxHealth;
	public int Health;
	public bool Invulnerable;
	public String DestIp;
	public int SourcePort;
	public int DestPort;

	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {
		Debug.Log ("Health = " + Health);
	}

	// Se llama antes del primer update (siempre despues de awake)
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("Sending UDP Packet to " + DestIp + " port " + DestPort);
			SendUdp(SourcePort, DestIp, DestPort, serialize());
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}

	public byte[] serialize()
	{
		Compressor compressor = new Compressor();
		Vector3 pos = this.transform.position;
        
		compressor.WriteNumber(this.Health, compressor.GetBitsRequired(this.MaxHealth));
		compressor.PutBit(this.Invulnerable);
		compressor.WriteFloat(pos.x, 100, 0, 0.1f);
		compressor.WriteFloat(pos.y, 100, 0, 0.1f);
		compressor.WriteFloat(pos.z, 100, 0, 0.1f);

		return compressor.GetBuffer();
	}

	public void deserialize(byte[] data)
	{
		Vector3 pos = new Vector3();
		Decompressor decompressor = new Decompressor(data);

		this.Health = decompressor.GetNumber(decompressor.GetBitsRequired(this.MaxHealth));
		this.Invulnerable = decompressor.GetBoolean();
		pos.x = decompressor.GetFloat(100, 0, 0.1f);
		pos.y = decompressor.GetFloat(100, 0, 0.1f);
		pos.z = decompressor.GetFloat(100, 0, 0.1f);
		this.gameObject.transform.position = pos;
	}

	public override string ToString()
	{
		return "Health: " + Health + "\nInvulnerable: " + Invulnerable + "\nPosition: " +
		       this.gameObject.transform.position;
	}
}
