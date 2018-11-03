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
	public static int SourcePort;
	public float time = 0f;

	public static int listenPort;
	public int idCount = 1;

	private Boolean hasData;
	private byte[] data;

	public Dictionary<int, PlayerSnapshot> players = new Dictionary<int, PlayerSnapshot>();
	public Dictionary<int, int> lastAcks = new Dictionary<int, int>();
	public Dictionary<Connection, int> connections = new Dictionary<Connection, int>();
	public Dictionary<int, List<InputKey>> actions = new Dictionary<int, List<InputKey>>();
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

		if (hasData)
		{
			ProccessData(data);
			hasData = false;
		}
		//TODO 
		/*foreach (int id in connections.Values)
		{
			UpdatePlayer(id);
		}*/



		if (time > snapRate && players.Count != 0)
		{
			time -= snapRate;
			if (players.Count != 0)
			{
				Debug.Log("Server Sending UDP Packet to " + DestIp + " port " + DestPort);
				byte[] bytes = players[1].serialize();
				SendUdp(SourcePort, DestIp, DestPort, bytes);
				
				//TODO SERIALIZE WORLD
				byte[] serializedWorld = null;
				foreach (Connection con  in connections.Keys)
				{
					SendUdp(SourcePort, con.srcIp.ToString(), con.srcPrt, serializedWorld);
				}
			}
		}
	}

	private void processPacket(byte[] package)
	{
		//processGameMessage(PacketQueue.GetInstance().PollPacket());

	}

	private void processConnect(GameMessage gm, Connection connection)
	{
		if (!connections.ContainsKey(connection))
		{
			EstablishConnection(connection);
		}
		else
		{
			ACK(connection, 1);
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
				hasData = true;
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


	private void EstablishConnection(Connection connection)
	{
		int id = idCount++;
		connections.Add(connection, id);
		lastAcks.Add(id, 1);
		Random random = new Random();
		int range = (int)(GlobalSettings.MaxPosition - GlobalSettings.MinPosition);

		Vector3 position = new Vector3(random.Next(range) + GlobalSettings.MinPosition
			, random.Next(range) + GlobalSettings.MinPosition
			, 0);
			
		players.Add(id, new PlayerSnapshot(position));
	}

	private void UpdatePlayer(int id)
	{
		List<InputKey> keys = actions[id];
		PlayerSnapshot ps = players[id];
		foreach (InputKey key in keys)
		{
			ps.apply(key);
		}
		players[id] = ps;
	}

	private void ACK(Connection connection, int ack)
	{
		//TODO crearmensajedetipoACK
		byte[] ackmsg = null;
		SendUdp(SourcePort, connection.srcIp.ToString(), connection.srcPrt, ackmsg);
	}

	private void ProccessData(byte[] data)
	{
		MessageType type = getType(data);
		//Remember to ignore the first byte
			
		switch (type)
		{
			case MessageType.ClientConnect:
				Debug.Log("CONNECT MESSAGE RECIEVED");
				break;
			case MessageType.PlayerSnapshot:
				Debug.Log("SNAP");
				break;
			default:
				break;
		}

	}
}
