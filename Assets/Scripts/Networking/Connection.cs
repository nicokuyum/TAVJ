using System.Net;

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
    }
}
