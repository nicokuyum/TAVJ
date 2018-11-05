using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
	static readonly object lockObject = new object();
	
	private List<GameMessage> outgoingMessages;
	
	public String DestIp;
	public static int DestPort = GlobalSettings.GamePort;
	public static int SourcePort = 8081;
	public static int listenPort = GlobalSettings.GamePort;
	
	private float time = 0.0f;
	private float acumTime = 0.0f;
	private float fps = GlobalSettings.Fps;
	private long frame = 0;
	private int subframe = 0;

	private ClientMessageHandler handler;
	private ReliableQueue rq;

	private Player player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Player>();
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
		subframe++;
		if (subframe == GlobalSettings.PrintingSubFrameRate)
		{
			subframe = 0;
		}
		
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

			foreach (var action in player.getActions())
			{
				outgoingMessages.Add(action);
			}
			
			player.getActions().Clear();
			
			foreach (var gm in outgoingMessages)
			{
				if (gm.isReliable())
				{
					rq.AddQueue((ReliableMessage)gm, frame);
				}
			}
			
			SnapshotHandler.GetInstance().updatePlayer(SnapshotHandler.GetInstance().getSnapshot(frame, subframe));
			
			outgoingMessages.AddRange(rq.MessageToResend(frame));
			Debug.Log("Outgoing messages size: " + outgoingMessages.Count);
			if (outgoingMessages.Count > 0)
			{
				Packet p = new Packet(outgoingMessages);
				//Debug.Log("p size = " + p.MessageCount);
				byte[] bytes = p.serialize();
				//Debug.Log("bytes = " + Encoding.Default.GetString(bytes));
				//Packet test = new Packet(bytes, null);
				//Debug.Log("test size = " + test.MessageCount);
				SendUdp(bytes);
				outgoingMessages.Clear();
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
