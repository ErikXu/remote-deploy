using System;
using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = CommandKey.Connect)]
    public class ConnectCommand : IAsyncCommand<PackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            if (session is ServerSession serverSession)
            {
                if (package.Content.Equals(ClientType.Web.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    serverSession.ClientType = ClientType.Web;
                }
                else if (package.Content.Equals(ClientType.Agent.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    serverSession.ClientType = ClientType.Agent;
                }
                else if (package.Content.Equals(ClientType.Short.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    serverSession.ClientType = ClientType.Short;
                }
                else
                {
                    serverSession.ClientType = ClientType.Unknown;
                }
            }

            await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connected}{Package.Terminator}"));
        }
    }
}