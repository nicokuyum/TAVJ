using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour {
	
	private TcpClient connection; 

	// Use this for initialization
	void Start () {
		Thread receive = new Thread (new ThreadStart(Listen)); 			
		receive.IsBackground = true;		
		receive.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Listen()
	{
	
	}
}
