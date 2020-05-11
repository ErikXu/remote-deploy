using System.Text;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace RemoteServer.Commands
{
    [Command(Key = "Connect")]
    public class ConnectCommand : IAsyncCommand<StringPackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, StringPackageInfo package)
        {
            await session.SendAsync(Encoding.UTF8.GetBytes("Command ls /root"));
            //await session.SendAsync(Encoding.UTF8.GetBytes("Disconnect"));
        }
    }
}