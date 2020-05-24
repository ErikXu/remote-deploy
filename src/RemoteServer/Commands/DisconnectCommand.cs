using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = CommandKey.Disconnect)]
    public class DisconnectCommand : IAsyncCommand<PackageInfo>
    {
        private ISessionContainer _sessionContainer;

        private readonly IServiceProvider _serviceProvider;

        public DisconnectCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            Console.WriteLine(package.Content);
            _sessionContainer = _serviceProvider.GetSessionContainer();
            var targetSession = _sessionContainer.GetSessions<ServerSession>(n => n.SessionID == package.Content).SingleOrDefault();
            if (targetSession != null)
            {
                await targetSession.CloseAsync();
                await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Disconnect} {package.Content} is disconnected{Package.Terminator}"));
            }
            else
            {
                await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Disconnect} {package.Content} is not found{Package.Terminator}"));
            }
        }
    }
}