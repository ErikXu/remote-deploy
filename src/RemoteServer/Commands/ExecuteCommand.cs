using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = "Execute")]
    public class ExecuteCommand : IAsyncCommand<PackageInfo>
    {
        private ISessionContainer _sessionContainer;

        private readonly IServiceProvider _serviceProvider;

        public ExecuteCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            _sessionContainer = _serviceProvider.GetSessionContainer();
            var sessions = _sessionContainer.GetSessions<ServerSession>(n => n.ClientType == ClientType.Agent).ToList();

            foreach (IAppSession serverSession in sessions)
            {
                await serverSession.SendAsync(Encoding.UTF8.GetBytes("Execute " + package.Content + Package.Terminator));
            }
        }
    }
}