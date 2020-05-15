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
    [Command(Key = "ListClient")]
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

            var sessions = _sessionContainer.GetSessions<ServerSession>().ToList();

            var clients = sessions.Select(n => new ClientInfo
            {
                SessionId = n.SessionID,
                Ip = n.Ip,
                Port = n.Port,
                ClientType = n.ClientType
            }).ToList();

            await session.SendAsync(Encoding.UTF8.GetBytes("ListClient " + JsonConvert.SerializeObject(clients) + Package.Terminator));
        }
    }
}