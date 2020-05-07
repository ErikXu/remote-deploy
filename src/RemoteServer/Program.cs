using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RemoteServer.Commands;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;

namespace RemoteServer
{
    class Program
    {
        static IHostBuilder CreateSocketServerBuilder()
        {
            return SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
                .UseCommand((commandOptions) =>
                {
                    commandOptions.AddCommand<ConnectCommand>();
                    //commandOptions.AddCommandAssembly(typeof(Program).GetTypeInfo().Assembly);
                })
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "TestServer" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", "4040" }
                    });
                })
                .ConfigureLogging((hostCtx, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                });
        }

        static async Task Main(string[] args)
        {
            var builder = CreateSocketServerBuilder();
            builder.UseSession<ServerSession>();
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
