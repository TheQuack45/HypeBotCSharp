using ChatSharp;
using ChatSharp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HypeBotCSharp
{
    class UtilFunc
    {
        public static bool IsHostnameValid(string ircHostname)
        {
            bool isValid = false;
            try
            {
                IPHostEntry ipHost = Dns.Resolve(ircHostname);
                isValid = true;
            } catch (SocketException e)
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
