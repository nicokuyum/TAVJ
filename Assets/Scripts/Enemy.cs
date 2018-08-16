using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class Enemy : MonoBehaviour
{
	static readonly object lockObject = new object();
	public int listenPort;
	private Boolean hasData;
	private String data;
	// Use this for initialization
	void Start () {
		Thread thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if (hasData)
		{
			lock (lockObject)
			{
				Debug.Log("Received message: " + data);
				data = "";
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
				data += Encoding.ASCII.GetString(receiveBytes);
				hasData = true;
			}
		}
	}
}