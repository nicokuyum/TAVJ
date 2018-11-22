using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/**
 * Single class with queue of all incoming packets
 */
public class PacketQueue
{
	public float packetLoss = 0.0f;
	private static readonly Object LockObject = new Object();
	private static PacketQueue _instance;
	
	// Only one packet queue per game instance
	private Queue<Packet> queue;

	private PacketQueue()
	{
		queue = new Queue<Packet>();
	}

	public static PacketQueue GetInstance()
	{
		return _instance ?? (_instance = new PacketQueue());
	}

	public void PushPacket(Packet p)
	{
		Random r = new Random();
		if (r.NextDouble() >= packetLoss)
		{
			lock (LockObject)
			{
				queue.Enqueue(p);
			}
		}
	}

	public Packet PollPacket()
	{
		Packet p = null;
		lock (LockObject)
		{

			if (queue.Count != 0)
			{
				p = queue.Dequeue();
			}
		}
		return p;
	}
	
}
