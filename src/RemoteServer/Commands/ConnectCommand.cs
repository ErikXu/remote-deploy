using System;
using System.Text;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = "Connect")]
    public class ConnectCommand : IAsyncCommand<PackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            if (session is ServerSession serverSession)
            {
                if (package.Content.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    serverSession.ClientType = ClientType.Web;
                }
                else if (package.Content.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                {
                    serverSession.ClientType = ClientType.Agent;
                }
                else
                {
                    serverSession.ClientType = ClientType.Unknown;
                }
            }
  
            await session.SendAsync(Encoding.UTF8.GetBytes("Command ls /root"));
            //await session.SendAsync(Encoding.UTF8.GetBytes("Disconnect"));
        }
    }
}