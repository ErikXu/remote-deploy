using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = CommandKey.ListClient)]
    public class ListClientCommand : IAsyncCommand<PackageInfo>
    {
        private ISessionContainer _sessionContainer;

        private readonly IServiceProvider _serviceProvider;

        public ListClientCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            _sessionContainer = _serviceProvider.GetSessionContainer();

            var sessions = _sessionContainer.GetSessions<ServerSession>().Where(n => n.ClientType != ClientType.Short).ToList();

            var clients = sessions.Select(n => new ClientInfo
            {
                SessionId = n.SessionID,
                Ip = n.Ip,
                Port = n.Port,
                ClientType = n.ClientType,
                ConnectTime = n.ConnectTime
            }).ToList();

            await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.ListClient} {JsonConvert.SerializeObject(clients)}{Package.Terminator}"));
        }
    }
}