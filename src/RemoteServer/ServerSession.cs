using System;
using System.Threading.Tasks;
using SuperSocket.Server;

namespace RemoteServer
{
    public class ServerSession : AppSession
    {
        public ClientType ClientType { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        protected override async ValueTask OnSessionConnectedAsync()
        {
            await base.OnSessionConnectedAsync();
        }

        protected override async ValueTask OnSessionClosedAsync(EventArgs e)
        {
            await base.OnSessionClosedAsync(e);
        }
    }

    public enum ClientType
    {
        Unknown,
        Web,
        Agent
    }
}