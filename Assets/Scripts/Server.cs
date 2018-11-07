using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Networking;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using Random = System.Random;


public class Server : MonoBehaviour
{
	static readonly object lockObject = new object();

	public static float snapRate;
	public static String DestIp;
	public static int DestPort;
	public static int SourcePort = 8081;
	public float time = 0f;

	private static int listenPort = GlobalSettings.GamePort;
	public int idCount = 1;
	public long frameNumber = 0;

	private Boolean hasData;
	private byte[] data;
	
	
	public List<PlayerSnapshot> playersnapshots = new List<PlayerSnapshot>();

	public Dictionary<int, PlayerSnapshot> players = new Dictionary<int, PlayerSnapshot>();
	public Dictionary<int, int> lastAcks = new Dictionary<int, int>();
	public Dictionary<Connection, int> connections = new Dictionary<Connection, int>();
	public Dictionary<int, ServerReliableQueue> rq = new Dictionary<int, ServerReliableQueue>();
	public Dictionary<int, HashSet<PlayerAction>> actions = new Dictionary<int, HashSet<PlayerAction>>();
	
	
	
	// Use this for initialization
	void Start()
	{
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;

		Packet packet = PacketQueue.GetInstance().PollPacket();
		//Debug.Log("PACKET IS NULL? " + packet ==null);
		while (packet != null)
		{
			Debug.Log("POR PROCESAR PAQUETE");
			//ProcessPacket(packet);
			//packet = PacketQueue.GetInstance().PollPacket();
		}

		//TODO 
		/*foreach (int id in connections.Values)
		{
			UpdatePlayer(id);
		}*/

		SendReliableMessages(frameNumber);


		if (time > snapRate && players.Count != 0)
		{
			frameNumber++;
			time -= snapRate;
			byte[] serializedWorld = SerializeWorld();
			foreach (Connection connection in connections.Keys)
			{
				//Debug.Log("SENDING WORLD");	
				SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, serializedWorld);
			}

			for (int i = 1; i < idCount; i++)
			{
				UpdatePlayer(i);
			}

		}
	}



	private void processConnect(ClientConnectMessage ccm, Connection connection)
	{
		Debug.Log("PROCESSING CONNECTION");
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
				Debug.Log("ENTRARON " + data.Length + " BYTES");
				PacketQueue.GetInstance().PushPacket(new Packet(data, connection));
				Debug.Log("ENTRO ALGO");
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
		//connections[connection] = id;
		lastAcks.Add(id, messageId);
		Random random = new Random();
		
		int range = (int)(GlobalSettings.MaxPosition - GlobalSettings.MinPosition);
		Vector3 position = new Vector3(random.Next(range) + GlobalSettings.MinPosition
			, random.Next(range) + GlobalSettings.MinPosition
			, 0);
		actions[id]= new HashSet<PlayerAction>();
		PlayerSnapshot ps = new PlayerSnapshot(id, position);
		rq.Add(id,new ServerReliableQueue(connection));
		players.Add(id, ps);
		playersnapshots.Add(ps);
		BroadCastConnectionMessage(id, playerName);
	}

	private void BroadCastConnectionMessage(int id, String playerName)
	{
		foreach (int playerId in players.Keys)
		{
			rq[playerId].AddQueue(new ClientConnectedMessage(id, playerName), frameNumber);
		}
	}
	

	private void UpdatePlayer(int id)
	{
		//Debug.Log("Updating Player   "  + id);
		HashSet<PlayerAction> playerActions = actions[id];
		
		PlayerSnapshot ps = players[id];
		ps.frameNumber++;

		foreach (PlayerAction action in playerActions)
		{
			Mover.GetInstance().ApplyAction(ps,action);
		}
		
	}

	private void SendAck(Connection connection, int ack)
	{
		//TODO crearmensajedetipoACK
		byte[] ackmsg = (new AckMessage(ack)).Serialize();

		List<GameMessage> gms = new List<GameMessage>();
		
		gms.Add( new AckMessage(ack));
		
		Packet packet = new Packet(gms);
		SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, packet.serialize());
	}

	private void ProcessPacket( Packet packet)
	{
		Debug.Log("PROCESANDO PAQUETEEEEE");
		foreach (GameMessage gm in packet.Messages)
		{
			Debug.Log("TYPE : " + gm.type().ToString() );
			switch (gm.type())
			{
				case MessageType.ClientConnect:
					processConnect((ClientConnectMessage)gm,packet.connection);
					break;
				case MessageType.PlayerInput:
					processInput((PlayerInputMessage)gm, packet.connection);
					break;
				case MessageType.Ack:
					processAck((AckMessage) gm, packet.connection);
					break;
				default:
					throw new NotImplementedException();
					break;
			}
			
			
			//TODO Procesar si es reliable SOLAMENTE si el ack es inferior al que mande
			if (gm.isReliable())
			{
				ReliableMessage rm = (ReliableMessage) gm;
				SendAck(packet.connection, rm._MessageId);
			}
		}

	}

	private byte[] SerializeWorld()
	{
		List<GameMessage> gms = new List<GameMessage>();
		foreach (PlayerSnapshot playerSnapshot in playersnapshots)// players.Values
		{
			gms.Add(new PlayerSnapshotMessage(playerSnapshot));
		}
		return (new Packet(gms)).serialize();
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
		if (lastAcks[id] < gm.ackid)
		{
			lastAcks[id] = gm.ackid;
			rq[id].ReceivedACK(gm.ackid);
		}
	}

	public void SendReliableMessages(long frameNumber)
	{
		foreach (KeyValuePair<int, ServerReliableQueue> entry in rq)
		{
			List<GameMessage> messagesToSend = entry.Value.MessageToResend(frameNumber);
			Packet packet = new Packet(messagesToSend);
			
			SendUdp(SourcePort, entry.Value.connection.srcIp.ToString(), GlobalSettings.GamePort, packet.serialize());
		}
	}
}
