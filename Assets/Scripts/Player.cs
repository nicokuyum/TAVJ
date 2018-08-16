using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour {

	public int health;
	public String destIp;
	public int sourcePort;
	public int destPort;

	// Se llama luego de haber sido constuido el GameObject y todos sus componentes
	void Awake () {
		Debug.Log ("Health = " + health);
	}

	// Se llama antes del primer update (siempre despues de awake)
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("Sending UDP Packet to " + destIp + " port " + destPort);
			SendUdp(sourcePort, destIp, destPort, Encoding.ASCII.GetBytes("Hello bitch"));
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}
}
