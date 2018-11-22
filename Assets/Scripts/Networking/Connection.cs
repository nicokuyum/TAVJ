using System;
using System.Net;
using UnityEngine;

namespace Networking
{
    public class Connection
    {
        public override bool Equals(object obj)
        {
            Connection con = (Connection) obj;
            return con.GetHashCode() == con.GetHashCode();
        }

        public override int GetHashCode()
        {
            return srcIp.GetHashCode();
        }

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
