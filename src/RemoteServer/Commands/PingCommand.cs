using System.Text;
using System.Threading.Tasks;
using RemoteCommon;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = CommandKey.Ping)]
    public class PingCommand : IAsyncCommand<PackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            await session.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Pong} {package.Content}{Package.Terminator}"));
        }
    }
}