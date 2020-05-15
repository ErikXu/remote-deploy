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
    [Command(Key = "ListAgent")]
    public class ListAgentCommand : IAsyncCommand<PackageInfo>
    {
        private ISessionContainer _sessionContainer;

        private readonly IServiceProvider _serviceProvider;

        public ListAgentCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            _sessionContainer = _serviceProvider.GetSessionContainer();

            var sessions = _sessionContainer.GetSessions<ServerSession>(n => n.ClientType == ClientType.Agent).ToList();

            var agents = sessions.Select(n => new AgentInfo
            {
                SessionId = n.SessionID,
                Ip = n.Ip,
                Port = n.Port,
                ConnectTime = n.ConnectTime
            }).ToList();

            await session.SendAsync(Encoding.UTF8.GetBytes("ListAgent " + JsonConvert.SerializeObject(agents) + Package.Terminator));
        }
    }
}