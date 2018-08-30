using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
	static readonly object lockObject = new object();

	public int listenPort;

	private Boolean hasData;
	private byte[] data;

	// Use this for initialization
	void Start () {
		Debug.Log("v1.156");
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (hasData)
		{
			lock (lockObject)
			{
				Player p = GameObject.Find("Player").GetComponent<Player>();
				p.deserialize(data);
				Debug.Log("Received player: " + p);
				hasData = false;
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
				data = (byte[]) receiveBytes.Clone();
				hasData = true;
			}
		}
	}
}
