using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour, Serializable<Player>
{
	public String username;
	public int id;
	public int MaxHealth;
	public int Health;
	public bool Invulnerable;
	public String DestIp;
	public int SourcePort;
	public int DestPort;

	private float time = 0.0f;
	private float acumTime = 0.0f;
	private float fps = 60.0f;
	private long frameNumber = 0l;

	private Queue<PlayerSnapshot> snaps = new Queue<PlayerSnapshot>();
	
	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {
//		Debug.Log ("Health = " + Health);
	}

	// Se llama antes del primer update (siempre despues de awake)
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		time += Time.deltaTime;
		acumTime += Time.deltaTime;
		
		if (Input.GetKey(KeyCode.W))
		{
			this.gameObject.transform.Translate(Vector3.forward * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.A))
		{
			this.gameObject.transform.Translate(Vector3.left * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.S))
		{
			this.gameObject.transform.Translate(Vector3.back * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.D))
		{
			this.gameObject.transform.Translate(Vector3.right * Time.deltaTime);
		}

		
		if (acumTime >= (1.0f/60.0f))
		{
			frameNumber++;
			Debug.Log(1.0f/fps);
			acumTime -= (1.0f/60.0f);
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
		Quaternion rotation = this.transform.rotation;
		compressor.WriteString(username);
		compressor.WriteNumber(frameNumber, compressor.GetBitsRequired(3600 * (long)fps ));
		compressor.WriteNumber(this.Health, compressor.GetBitsRequired(this.MaxHealth));
		compressor.PutBit(this.Invulnerable);
		compressor.WriteFloat(pos.x, 100, 0, 0.1f);
		compressor.WriteFloat(pos.y, 100, 0, 0.1f);
		compressor.WriteFloat(pos.z, 100, 0, 0.1f);
//		compressor.WriteFloat(rotation.w);
		return compressor.GetBuffer();
	}

	public void deserialize(byte[] data)
	{
		Vector3 pos = new Vector3();
		Decompressor decompressor = new Decompressor(data);
		this.username = decompressor.GetString();
		this.frameNumber = decompressor.GetNumber(3600 * (long) fps);
		this.Health = decompressor.GetNumber(this.MaxHealth);
		this.Invulnerable = decompressor.GetBoolean();
		pos.x = decompressor.GetFloat(100, 0, 0.1f);
		pos.y = decompressor.GetFloat(100, 0, 0.1f);
		pos.z = decompressor.GetFloat(100, 0, 0.1f);
		this.gameObject.transform.position = pos;
	}
	
	 

	public override string ToString()
	{
		return "Health: " + Health + "\nInvulnerable: " + Invulnerable + " 		Position: " +
		       this.gameObject.transform.position;
	}
}
