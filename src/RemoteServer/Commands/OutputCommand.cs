using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = "Output")]
    public class OutputCommand : IAsyncCommand<PackageInfo>
    {
        private ISessionContainer _sessionContainer;

        private readonly IServiceProvider _serviceProvider;

        public OutputCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            _sessionContainer = _serviceProvider.GetSessionContainer();
            var sessions = _sessionContainer.GetSessions<ServerSession>(n => n.ClientType == ClientType.Web).ToList();

            foreach (IAppSession serverSession in sessions)
            {
                await serverSession.SendAsync(Encoding.UTF8.GetBytes(package.Content + Package.Terminator));
            }
        }
    }
}