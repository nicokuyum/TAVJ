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

	private Dictionary<int, Player> otherPlayers;

	public GameObject prefab;
	public String DestIp;
	public String playerName;
	public bool prediction;
	public static int DestPort = GlobalSettings.ServerPort;
	public static int SourcePort = 8081;
	public static int listenPort = GlobalSettings.GamePort;
	
	private float time = 0.0f;
	private float acumTime = 0.0f;
	private float fps = GlobalSettings.Fps;
	private long frame = 0;
	private int subframe = 0;

	private ClientMessageHandler handler;
	private ReliableQueue rq { get; set; }

	private Player player { get; set; }

	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player").GetComponent<Player>();
		player.name = playerName;
		outgoingMessages = new List<GameMessage>();
		rq = new ReliableQueue();
		outgoingMessages.Add(new ClientConnectMessage(playerName, time));
		otherPlayers = new Dictionary<int, Player>();
		handler = new ClientMessageHandler(this);
		SnapshotHandler.GetInstance().otherPlayers = this.otherPlayers;
		SnapshotHandler.GetInstance().self = this.player;
		SnapshotHandler.GetInstance().prediction = this.prediction;
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

			foreach (var action in player.getActions())
			{
				SnapshotHandler.GetInstance().AddActionForPrediction(action);
				outgoingMessages.Add(action);
			}
			
			player.getActions().Clear();
			
			foreach (var gm in outgoingMessages)
			{
				if (gm.isReliable())
				{
					//TODO check if there already is time sync between client and server (es necesario que lo haya?)
					rq.AddQueue((ReliableMessage)gm, time);
				}
			}

			//TODO Fijarse de no empezar a pedir esto hasta que se haya bufferizado un poco
			Dictionary<int,PlayerSnapshot> currentSnapshot = SnapshotHandler.GetInstance().getSnapshot(time);
			if (currentSnapshot != null)
			{
				SnapshotHandler.GetInstance().updatePlayer(currentSnapshot);
			}
			
			outgoingMessages.AddRange(rq.MessageToResend(time));
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

	public List<GameMessage> getOutgoingMessages()
	{
		return this.outgoingMessages;
	}

	public GameObject getPlayerPrefab()
	{
		return this.prefab;
	}

	public Dictionary<int, Player> getOtherPlayers()
	{
		return otherPlayers;
	}

	public void setTime(float time)
	{
		this.time = time;
	}

	public Player getPlayer()
	{
		return this.player;
	}

	public ReliableQueue GetReliableQueue()
	{
		return this.rq;
	}
}
