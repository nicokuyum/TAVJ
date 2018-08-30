using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
	static readonly object lockObject = new object();

	public static float snapRate;
	public static String DestIp;
	public static int DestPort;
	public static int SourcePort;
	public float time = 0f;
	
	public static int listenPort;
	public int idCount = 1;

	public Dictionary<int,Player> players = new Dictionary<int,Player>();
	
	// Use this for initialization
	void Start () {
		//Thread thread = new Thread(new ThreadStart(ThreadMethod));
		//thread.Start();
	}
	
	// Update is called once per frame
	void Update ()
	{
		time += Time.deltaTime;
		if (time > snapRate && players.Count != 0)
		{
			time -= snapRate;
			if (players.Count != 0)
			{
				Debug.Log("Server Sending UDP Packet to " + DestIp + " port " + DestPort);
				byte[] bytes = players[1].serialize();
				SendUdp(SourcePort, DestIp, DestPort, bytes);
			}
		}
		
		
	}

	

	private void ThreadMethod()
	{
		UdpClient udpClient = new UdpClient(listenPort);

		while (true)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
			byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

			lock (lockObject)
			{
				//data += Encoding.ASCII.GetString(receiveBytes);
				//hasData = true;
			}
			
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}
}
