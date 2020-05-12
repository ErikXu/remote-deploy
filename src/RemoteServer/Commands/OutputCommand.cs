using System;
using System.Linq;
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

        private IServiceProvider _serviceProvider;

        public OutputCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            _sessionContainer = _serviceProvider.GetSessionContainer();
            var sessions = _sessionContainer.GetSessions<ServerSession>().ToList();
            Console.WriteLine(package.Key);
        }
    }
}