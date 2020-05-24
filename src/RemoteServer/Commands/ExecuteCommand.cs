using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using RemoteProto;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = CommandKey.Execute)]
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
            var command = Command.Parser.ParseFrom(Encoding.UTF8.GetBytes(package.Content));

            _sessionContainer = _serviceProvider.GetSessionContainer();
            var sessions = _sessionContainer.GetSessions<ServerSession>(n => n.ClientType == ClientType.Agent && n.Ip == command.Ip).ToList();

            foreach (IAppSession serverSession in sessions)
            {
                await serverSession.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Execute} {package.Content}{Package.Terminator}"));
            }

            await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Execute} Started{Package.Terminator}"));
        }
    }
}