using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
	static readonly object lockObject = new object();
	
	private List<GameMessage> outgoingMessages;
	
	public static String DestIp;
	public static int DestPort;
	public static int SourcePort;
	public static int listenPort;
	
	private float time = 0.0f;
	private float acumTime = 0.0f;
	private float fps = GlobalSettings.Fps;

	private ClientMessageHandler handler;

	// Use this for initialization
	void Start () {
		handler = new ClientMessageHandler();
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		acumTime += Time.deltaTime;
		
		if (acumTime >= (1.0f/fps))
		{
			acumTime -= (1.0f/fps);
			Packet p = PacketQueue.GetInstance().PollPacket();
			while (p != null)
			{
				foreach (var gm in p.Messages)
				{
					handler.Handle(gm);
				}
				p = PacketQueue.GetInstance().PollPacket();
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
				PacketQueue.GetInstance().PushPacket(new Packet(receiveBytes, null));
			}
			
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}
}
