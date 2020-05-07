using System;
using System.Threading.Tasks;
using SuperSocket.Server;

namespace RemoteServer
{
    public class ServerSession : AppSession
    {
        protected override async ValueTask OnSessionConnectedAsync()
        {
            await base.OnSessionConnectedAsync();
        }

        protected override async ValueTask OnSessionClosedAsync(EventArgs e)
        {
            await base.OnSessionClosedAsync(e);
        }
    }
}