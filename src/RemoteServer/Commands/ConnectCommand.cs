using System.Text;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace RemoteServer.Commands
{
    [Command("connect")]
    public class ConnectCommand : IAsyncCommand<StringPackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, StringPackageInfo package)
        {
            await session.SendAsync(Encoding.UTF8.GetBytes("Connected"));
        }
    }
}