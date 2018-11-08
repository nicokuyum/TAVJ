using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Networking;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.VersionControl;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;


public class Server : MonoBehaviour
{
	static readonly object lockObject = new object();

	public static float snapRate = GlobalSettings.ServerSendRate;
	public static int SourcePort = 8081;
	
	private float time = 0f;
	private float acumTime = 0f;
	
	private static int listenPort = GlobalSettings.ServerPort;
	public int idCount = 1;
	public float PacketLoss = 0.0f;

	private Boolean hasData;
	private byte[] data;


	public GameObject prefab;
	
	
	//All Snapshots
	public List<PlayerSnapshot> playersnapshots = new List<PlayerSnapshot>();
	
	//ID to Player mapping
	public Dictionary<int, PlayerSnapshot> players = new Dictionary<int, PlayerSnapshot>();
	
	//ID to Last ACK mapping
	public Dictionary<int, int> lastAcks = new Dictionary<int, int>();
	
	//Connection to ID mapping
	public Dictionary<Connection, int> connections = new Dictionary<Connection, int>();
	
	//ID to ReliableQueue mapping
	public Dictionary<int, ServerReliableQueue> rq = new Dictionary<int, ServerReliableQueue>();
	
	//ID to PlayerActions
	public Dictionary<int, HashSet<PlayerAction>> actions = new Dictionary<int, HashSet<PlayerAction>>();
	
	//ID to stored ReliableMessageIdStored
	public Dictionary<int, SortedList<ReliableMessage, bool>> bufferedMessages =
		new Dictionary<int, SortedList<ReliableMessage, bool>>();
	
	
	// Use this for initialization
	void Start()
	{
		PacketQueue.GetInstance().packetLoss = PacketLoss;
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;
		acumTime += Time.deltaTime;

		Packet packet = PacketQueue.GetInstance().PollPacket();
		//Debug.Log("PACKET IS NULL? " + packet ==null);
		while (packet != null)
		{
			//Debug.Log("POR PROCESAR PAQUETE");
			ProcessPacket(packet);
			packet = PacketQueue.GetInstance().PollPacket();
		}

		//TODO 
		/*foreach (int id in connections.Values)
		{
			UpdatePlayer(id);
		}*/

		SendReliableMessages();

		for (int i = 1; i < idCount; i++)
		{
			UpdatePlayer(i, Time.deltaTime);
		}
		
		if (acumTime >= (1.0f/ snapRate) && players.Count != 0)
		{
			//Debug.Log(time);
			while (acumTime > (1.0f/snapRate))
			{
				acumTime -= (1.0f/snapRate);
			}
			byte[] serializedWorld = SerializeWorld();
			foreach (Connection connection in connections.Keys)
			{
				//Debug.Log("SENDING WORLD");	
				SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, serializedWorld);
			}
		}
	}



	private void processConnect(ClientConnectMessage ccm, Connection connection)
	{
		if (!connections.ContainsKey(connection))
		{
			EstablishConnection(connection, ccm._MessageId, ccm.name);
		}

	}


	private void ThreadMethod()
	{
		UdpClient udpClient = new UdpClient(listenPort);

		while (true)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			
			byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
			
			Connection connection = new Connection(RemoteIpEndPoint.Address,RemoteIpEndPoint.Port);
			lock (lockObject)
			{
				data = (byte[]) receiveBytes.Clone();
				PacketQueue.GetInstance().PushPacket(new Packet(data, connection));
			}
		}
	}
	
	static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
	{
		using (UdpClient c = new UdpClient(srcPort))
			c.Send(data, data.Length, dstIp, dstPort);
	}

	static MessageType getType(byte[] data)
	{
		Decompressor decompressor = new Decompressor(data);
		return (MessageType) decompressor.GetNumber(Enum.GetValues(typeof(MessageType)).Length);
	}


	private void EstablishConnection(Connection connection, int messageId, String playerName)
	{
		int id = idCount++;
		connections.Add(connection, id);
		lastAcks.Add(id, messageId);

		
		GameObject go = Instantiate(prefab);
		ServerPlayer newPlayer = go.GetComponent<ServerPlayer>();
		PlayerSnapshot ps = new PlayerSnapshot(id, newPlayer, time);
		
		Debug.Log("CREATED AT" + ps.position.x + " " + ps.position.y  + " " + ps.position.z);
		rq.Add(id,new ServerReliableQueue(connection));
		actions[id] = new HashSet<PlayerAction>();
		bufferedMessages.Add(id, new SortedList<ReliableMessage, bool>());
		players.Add(id, ps);
		playersnapshots.Add(ps);
		Debug.Log("Broadcast de " + playerName);
		BroadCastConnectionMessage(id, playerName);
	}

	private void BroadCastConnectionMessage(int id, String playerName)
	{
		foreach (int playerId in players.Keys)
		{
			rq[playerId].AddQueue(new ClientConnectedMessage(id, playerName, time), time);
		}
	}
	

	private void UpdatePlayer(int id, float deltaTime)
	{
		//Debug.Log("Updating Player   "  + id);
		HashSet<PlayerAction> playerActions = actions[id];
		
		PlayerSnapshot ps = players[id];

		ps._TimeStamp = time;


		foreach (PlayerAction action in playerActions)
		{
			Mover.GetInstance().ApplyAction(ps,action, deltaTime);
		}
		//Debug.Log(ps.position.x  + " " + ps.position.z);
	}

	private void SendAck(Connection connection, int ack)
	{
		
		Debug.Log("Sending ACK  " + ack);

		List<GameMessage> gms = new List<GameMessage>();
		
		gms.Add( new AckMessage(ack));
		
		Packet packet = new Packet(gms);
		SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, packet.serialize());
	}

	private void ProcessPacket( Packet packet)
	{
		foreach (GameMessage gm in packet.Messages)
		{
			
			//Debug.Log("GM type : " + gm.type().ToString() );
			if (gm.isReliable())
			{
				if (gm.type() == MessageType.ClientConnect)
				{
					processConnect((ClientConnectMessage)gm, packet.connection);
				}
				else if (lastAcks[connections[packet.connection]] < ((ReliableMessage) gm)._MessageId)
				{
					ProcessMessage(gm, packet.connection);
				}
				SendAck(packet.connection, ((ReliableMessage) gm)._MessageId);
				lastAcks[connections[packet.connection]] = ((ReliableMessage) gm)._MessageId;

			}
			else
			{
				ProcessMessage(gm,packet.connection);
			}
			
			
			//TODO Procesar si es reliable SOLAMENTE si el ack es inferior al que mande
		/*	if (gm.isReliable())
			{
				ReliableMessage rm = (ReliableMessage) gm;
				SendAck(packet.connection, rm._MessageId);
			}*/
		}

	}

	private void ProcessMessage(GameMessage gm, Connection connection)
	{			
		Debug.Log(gm.type().ToString());
		switch (gm.type())
		{
			case MessageType.ClientConnect:
				processConnect((ClientConnectMessage)gm, connection);
				break;
			case MessageType.PlayerInput:
				processInput((PlayerInputMessage)gm, connection);
				break;
			case MessageType.Ack:
				processAck((AckMessage) gm, connection);
				break;
			default:
				throw new NotImplementedException();
				break;
		}
	}

	private byte[] SerializeWorld()
	{
		/*List<GameMessage> gms = new List<GameMessage>();
		foreach (PlayerSnapshot playerSnapshot in playersnapshots)// players.Values
		{
			gms.Add(new PlayerSnapshotMessage(playerSnapshot));
			Debug.Log(playerSnapshot.position.x +  " " + playerSnapshot.position.z + " " + playerSnapshot.position.y);
		}
		
		Debug.Log("Mundo de : " + gms.Count );
		return (new Packet(gms)).serialize();*/
		List<GameMessage> l = new List<GameMessage>();
		l.Add(new WorldSnapshotMessage(playersnapshots, time));
		return (new Packet(l)).serialize();
	}

	
	
	private void processInput(PlayerInputMessage inputMessage, Connection connection)
	{	
		int id = connections[connection];
		//Debug.Log(inputMessage.Action);
		switch (inputMessage.Action)
		{
			case PlayerAction.StartMoveForward:
				actions[id].Add(PlayerAction.StartMoveForward);
				break;
			case PlayerAction.StartMoveRight:
				actions[id].Add(PlayerAction.StartMoveRight);
				break;
			case PlayerAction.StartMoveBack:
				actions[id].Add(PlayerAction.StartMoveBack);
				break;
			case PlayerAction.StartMoveLeft:
				actions[id].Add(PlayerAction.StartMoveLeft);
				break;
			case PlayerAction.StopMoveForward:
				actions[id].Remove(PlayerAction.StartMoveForward);
				break;
			case PlayerAction.StopMoveRight:
				actions[id].Remove(PlayerAction.StartMoveRight);
				break;
			case PlayerAction.StopMoveBack:
				actions[id].Remove(PlayerAction.StartMoveBack);
				break;
			case PlayerAction.StopMoveLeft:
				actions[id].Remove(PlayerAction.StartMoveLeft);
				break;
			case PlayerAction.Shoot:
				actions[id].Add(PlayerAction.Shoot);
				break;
			default:
				break;
		}
	}

	public void processAck(AckMessage gm, Connection connection)
	{
		int id = connections[connection];
		rq[id].ReceivedACK(gm.ackid);
	}

	public void SendReliableMessages()
	{
		foreach (KeyValuePair<int, ServerReliableQueue> entry in rq)
		{
			List<GameMessage> messagesToSend = entry.Value.MessageToResend(time);
			if (messagesToSend.Count > 0)
			{
				Packet packet = new Packet(messagesToSend);
				SendUdp(SourcePort, entry.Value.connection.srcIp.ToString(), GlobalSettings.GamePort, packet.serialize());
			}
		}
	}

	public void processPendingReliables(Connection connection)
	{
		
	}
	
}
