using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Connection
{
    public int srcPrt;
    public IPAddress srcIp;

    public Connection(IPAddress ip, int port)
    {
        srcIp = ip;
        srcPrt = port;
    }
}
