using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class ServerReliableQueue : ReliableQueue
{

    public Connection connection;

    public ServerReliableQueue(Connection connection) : base()
    {
        this.connection = connection;
    }
    
    
}
