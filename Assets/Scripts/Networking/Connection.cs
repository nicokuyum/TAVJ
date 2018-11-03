using System;
using System.Net;
using UnityEngine;

namespace Networking
{
    public class Connection
    {
        public int srcPrt;
        public IPAddress srcIp;

        public Connection(IPAddress ip, int port)
        {
            srcIp = ip;
            srcPrt = port;
        }

        public String toString()
        {
            return srcIp.ToString() + " : " + srcPrt;
        }
    }
    

}
