using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CRM.Common
{
    public class RequestStatus
    {
        public string StatusCode = "0";
        public string Message = "Request processed Successfully";
    }
    public class PagingHelper
    {
        public static int blockSize = 10;
        public static int StartIndex(int blockNumber)
        {
            return (blockNumber - 1) * blockSize;
        }
    }
    public class GeneralMethods
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string ipAddress = "";

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }
            return ipAddress;
        }
    }
}
