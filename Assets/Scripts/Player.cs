using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour
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
			byte[] bytes = {1, 2, 3, 4};
			SendUdp(SourcePort, DestIp, DestPort, bytes);
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}
	
	public byte[] compressPlayer(Player player)
	{
		Compressor compressor = new Compressor();
		Vector3 pos = this.transform.position;
        
		compressor.WriteNumber(player.Health, compressor.GetBitsRequired(player.MaxHealth));
		compressor.PutBit(player.Invulnerable);
		compressor.WriteFloat(pos.x, 100, 0, 0.1f);
		compressor.WriteFloat(pos.y, 100, 0, 0.1f);
		compressor.WriteFloat(pos.z, 100, 0, 0.1f);

		return compressor.GetBuffer();
	}
}
