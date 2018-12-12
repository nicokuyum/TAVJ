using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Networking;
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
	private float inputTime = 0f;
	public long lag_ms = 0L;
	
	private static int listenPort = GlobalSettings.ServerPort;
	public int idCount = 1;
	public float PacketLoss = 0.0f;

	private Boolean hasData;
	private byte[] data;

	private ServerMessageHandler MessageHandler;


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
	public Dictionary<int, List<PlayerInputMessage>> actions = new Dictionary<int, List<PlayerInputMessage>>();
	
	//ID to stored ReliableMessageIdStored
	public Dictionary<int, SortedList<int, ReliableMessage>> bufferedMessages =
		new Dictionary<int, SortedList<int, ReliableMessage>>();

	//ID to Username
	public Dictionary<int, String> usernames = new Dictionary<int, string>();
	
	
	
	// Use  this for initialization
	void Start()
	{
		PacketQueue.GetInstance().lag_ms = lag_ms;
		PacketQueue.GetInstance().packetLoss = PacketLoss;
		MessageHandler = new ServerMessageHandler(this);
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
		SimulateConnections();
	}

	private void SimulateConnections()
	{

		while (idCount <= GlobalSettings.AIPlayers)
		{
			AI.add(idCount);
			EstablishAIConnection(new Connection(new IPAddress(52351234123+idCount), 5656), 1, "AIPlayer" + idCount);
		}
	}

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;
		acumTime += Time.deltaTime;
		inputTime += Time.deltaTime;

		Packet packet = PacketQueue.GetInstance().PollPacket();
		while (packet != null)
		{
			ProcessPacket(packet);
			packet = PacketQueue.GetInstance().PollPacket();
		}

		SendReliableMessages();

		SerializeWorld();


		
		for (int i = 1; i < idCount; i++)
		{
			UpdatePlayer(i, 1.0f / GlobalSettings.Fps);
			if (i <= GlobalSettings.AIPlayers)
			{
				AI.act(players[i]);
			}
		}
		
		if (acumTime >= (1.0f/ snapRate) && players.Count != 0)
		{
			while (acumTime > (1.0f/snapRate))
			{
				acumTime -= (1.0f/snapRate);
			}
			byte[] serializedWorld = SerializeWorld();
			foreach (Connection connection in connections.Keys)
			{
				if (connections[connection] > GlobalSettings.AIPlayers)
				{
					SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, serializedWorld);
				}
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


	public void EstablishAIConnection(Connection connection, int messageId, String playerName)
	{
		int id = idCount++;
		connections.Add(connection, id);
		lastAcks.Add(id, messageId);

		
		GameObject go = Instantiate(prefab);
		ServerPlayer newPlayer = go.GetComponent<ServerPlayer>();
		PlayerSnapshot ps = new PlayerSnapshot(id, newPlayer, time);
		ps.position = new Vector3(0,2,0);
		ps.player.transform.position = new Vector3(0,2,0);
		rq.Add(id,new ServerReliableQueue(connection, GlobalSettings.ReliableTimeout));
		actions[id] = new List<PlayerInputMessage>();
		bufferedMessages.Add(id, new SortedList<int, ReliableMessage>());
		players.Add(id, ps);
		usernames.Add(id, playerName);
		playersnapshots.Add(ps);
	}
	
	public void EstablishConnection(Connection connection, int messageId, String playerName)
	{
		int id = idCount++;
		connections.Add(connection, id);
		lastAcks.Add(id, messageId);

		
		GameObject go = Instantiate(prefab);
		ServerPlayer newPlayer = go.GetComponent<ServerPlayer>();
		PlayerSnapshot ps = new PlayerSnapshot(id, newPlayer, time);
		
		rq.Add(id,new ServerReliableQueue(connection, GlobalSettings.ReliableTimeout));
		actions[id] = new List<PlayerInputMessage>();
		bufferedMessages.Add(id, new SortedList<int, ReliableMessage>());
		players.Add(id, ps);
		usernames.Add(id, playerName);
		playersnapshots.Add(ps);
		NotifiyPreviousConnections(connection, id);
		BroadCastConnectionMessage(id, playerName);
	}

	private void NotifiyPreviousConnections(Connection connection, int id)
	{
		
		foreach (KeyValuePair<Connection,int> keyValuePair in connections)
		{
			if (keyValuePair.Value != id)
			{
				Debug.Log("Notifying about player : " + keyValuePair.Value);
				rq[id].AddQueueWithTimeout(new ClientConnectedMessage(keyValuePair.Value, 
					usernames[keyValuePair.Value],time, true),time);
			}
		}
	}

	private void BroadCastConnectionMessage(int id, String playerName)
	{
		foreach (int playerId in players.Keys)
		{
			if (playerId > GlobalSettings.AIPlayers)
			{
				ClientConnectedMessage cm = new ClientConnectedMessage(id, playerName, time, true);
				rq[playerId].AddQueueWithTimeout(cm, time);
			}
		}
	}
	

	private void UpdatePlayer(int id, float time)
	{
		
		List<PlayerInputMessage> playerActions = actions[id];
		
		PlayerSnapshot ps = players[id];

		ps._TimeStamp = this.time;


		while (playerActions.Count>0)
		{
			PlayerInputMessage mssg = playerActions[0];
			playerActions.RemoveAt(0);
			Mover.GetInstance().ApplyAction(ps, mssg.Action, time);
			ps.lastId = mssg._MessageId;
		}

	}

	private void SendAck(Connection connection, int ack)
	{
		
		List<GameMessage> gms = new List<GameMessage>();
		
		gms.Add( new AckMessage(ack));
		
		Packet packet = new Packet(gms);
		SendUdp(SourcePort, connection.srcIp.ToString(), GlobalSettings.GamePort, packet.serialize());
	}

	private void ProcessPacket( Packet packet)
	{
		bool reliableFlag = false;
		

		
		foreach (GameMessage gm in packet.Messages)
		{
			if (gm.isReliable())
			{
				if (gm.type() == MessageType.PlayerInput)
				{
					PlayerInputMessage rm = (PlayerInputMessage) gm;
				}
				reliableFlag = true;
				int id = ((ReliableMessage) gm)._MessageId;
				if (gm.type() == MessageType.ClientConnect)
				{
					processConnect((ClientConnectMessage)gm, packet.connection);
				}
				else if (lastAcks[connections[packet.connection]] == id - 1 )
				{
					MessageHandler.ProcessMessage(gm, packet.connection);
					lastAcks[connections[packet.connection]] = ((ReliableMessage) gm)._MessageId;
					processBufferedMessages(packet.connection);
				}
				else if (lastAcks[connections[packet.connection]] < id)
				{
					bufferMessage((ReliableMessage) gm, packet.connection);
				}
			}
			else
			{
				MessageHandler.ProcessMessage(gm,packet.connection);
			}
		}
		if (reliableFlag)
		{
			SendAck(packet.connection, lastAcks[connections[packet.connection]]);
		}
	}


	private byte[] SerializeWorld()
	{
		List<GameMessage> l = new List<GameMessage>();
		l.Add(new WorldSnapshotMessage(playersnapshots, time));
		return (new Packet(l)).serialize();
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

	public void bufferMessage(ReliableMessage rm, Connection connection)
	{
		int userId = connections[connection];
		
		if(!bufferedMessages[userId].ContainsKey(rm._MessageId))
		{
			bufferedMessages[userId].Add(rm._MessageId, rm);
		}
	}
	
	public void processBufferedMessages(Connection connection)
	{
		int userId = connections[connection];

		while (bufferedMessages[userId].ContainsKey(lastAcks[userId] + 1))
		{
			ReliableMessage rm = bufferedMessages[userId][lastAcks[userId]+1];
			lastAcks[userId] = rm._MessageId;
			bufferedMessages[userId].Remove(rm._MessageId);
			MessageHandler.ProcessMessage((GameMessage)rm, connection);
		}
	}
	
}
