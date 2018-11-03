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
	
	public String DestIp;
	public int DestPort;
	public int SourcePort;
	public int listenPort;
	
	private float time = 0.0f;
	private float acumTime = 0.0f;
	private float fps = GlobalSettings.Fps;
	private long frame = 0;

	private ClientMessageHandler handler;
	private ReliableQueue rq;

	private Player player;

	// Use this for initialization
	void Start () {
		outgoingMessages = new List<GameMessage>();
		rq = new ReliableQueue();
		outgoingMessages.Add(new ClientConnectMessage("asdf"));
		handler = new ClientMessageHandler(rq);
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		acumTime += Time.deltaTime;
		
		if (acumTime >= (1.0f/fps))
		{
			frame++;
			acumTime -= (1.0f/fps);
			Packet packet = PacketQueue.GetInstance().PollPacket();
			while (packet != null)
			{
				foreach (var gm in packet.Messages)
				{
					handler.Handle(gm);
				}
				packet = PacketQueue.GetInstance().PollPacket();
			}
			
			foreach (var gm in outgoingMessages)
			{
				if (gm.isReliable())
				{
					Debug.Log("Adding to rq");
					rq.AddQueue(gm, frame);
				}
			}
			outgoingMessages.AddRange(rq.MessageToResend(frame));
			Packet p = new Packet(outgoingMessages);
			SendUdp(p.serialize());
			outgoingMessages.Clear();
			Debug.Log("Outgoing messages size: " + outgoingMessages.Count);
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
				Debug.Log("Pushing packet");
				PacketQueue.GetInstance().PushPacket(new Packet(receiveBytes, null));
			}
			
		}
	}
	
	private void SendUdp(byte[] data)
	{
		using (UdpClient c = new UdpClient(SourcePort))
			c.Send(data, data.Length, DestIp, DestPort);
	}
}
