using System;
using System.Net;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket.Server;

namespace RemoteServer
{
    public class ServerSession : AppSession
    {
        public ClientType ClientType { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public DateTime ConnectTime { get; set; }

        protected override async ValueTask OnSessionConnectedAsync()
        {
            ConnectTime = DateTime.UtcNow;
            if (RemoteEndPoint is IPEndPoint remoteIpEndPoint)
            {
                Ip = remoteIpEndPoint.Address.ToString();
                Port = remoteIpEndPoint.Port;
            }
            await base.OnSessionConnectedAsync();
        }

        protected override async ValueTask OnSessionClosedAsync(EventArgs e)
        {
            await base.OnSessionClosedAsync(e);
        }
    }
}