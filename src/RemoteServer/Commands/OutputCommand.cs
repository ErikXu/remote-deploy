using System;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Command;

namespace RemoteServer.Commands
{
    [Command(Key = "Output")]
    public class OutputCommand : IAsyncCommand<PackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, PackageInfo package)
        {
            Console.WriteLine(package.Key);
        }
    }
}