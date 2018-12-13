using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using Random = System.Random;

/**
 * Single class with queue of all incoming packets
 */
public class PacketQueue
{
	/*public float packetLoss = 0.0f;
	public long lag_ms = 0L;
	private static readonly Object LockObject = new Object();
	private static PacketQueue _instance;
	
	// Only one packet queue per game instance
	private SortedList<long, Packet> queue;

	private PacketQueue()
	{
		queue = new SortedList<long, Packet>();
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
			long Timestamp = new DateTimeOffset(DateTime.UtcNow).ToFileTime();
			lock (LockObject)
			{
				queue.Add(Timestamp, p);
			}
		}
	}

	public Packet PollPacket()
	{
		Packet p = null;
		lock (LockObject)
		{
			long CurrentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToFileTime();	
			if (queue.Count != 0 && CurrentTimestamp > queue.First().Key + lag_ms)
			{
				p = queue.First().Value;
				queue.RemoveAt(0);
			}
		}
		return p;
	}*/
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
