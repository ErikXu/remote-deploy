using System;
using System.Threading.Tasks;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace RemoteServer.Commands
{
    [Command(Key = "Output")]
    public class OutputCommand : IAsyncCommand<StringPackageInfo>
    {
        public async ValueTask ExecuteAsync(IAppSession session, StringPackageInfo package)
        {
            Console.WriteLine(package.Body);
        }
    }
}