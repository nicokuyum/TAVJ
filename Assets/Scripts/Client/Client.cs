using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
	static readonly object lockObject = new object();

	private List<GameMessage> outgoingMessages;

	private Dictionary<int, ServerPlayer> otherPlayers;

	public GameObject prefab;
	public GameObject otherPlayersPrefab;
	public GameObject grenadePrefab;
	
	public String DestIp;
	public String playerName;
	public bool prediction;
	public static int DestPort = GlobalSettings.ServerPort;
	public static int SourcePort = 8077;
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
		rq = new ReliableQueue(GlobalSettings.ReliableTimeout);
		rq.AddQueueWithTimeout(new ClientConnectMessage(playerName, time, true),0);
		otherPlayers = new Dictionary<int, ServerPlayer>();
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

			foreach (var action in player.toSend)
			{
				outgoingMessages.Add(action);
			}
			player.toSend.Clear();
			
			foreach (var gm in outgoingMessages)
			{
				if (gm.isReliable())
				{
					if (MessageType.PlayerInput.Equals(gm.type()))
					{
						rq.AddQueueWithOutTimeout((ReliableMessage)gm);
					}
					else
					{
						rq.AddQueueWithTimeout((ReliableMessage)gm, time);					
					}
				}
			}

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
				byte[] bytes = p.serialize();
				SendUdp(bytes);
				outgoingMessages.Clear();
			}
		}
	}

	public void launchGrenade(GrenadeLaunchMessage glm)
	{
		GameObject go = Instantiate(grenadePrefab);
		go.GetComponent<Grenade>().Launch(glm.position, glm.direction);
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

	public Dictionary<int, ServerPlayer> getOtherPlayers()
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
