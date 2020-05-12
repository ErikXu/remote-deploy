using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RemoteCommon;
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
            return SuperSocketHostBuilder.Create<PackageInfo, RemoteCommon.CommandLinePipelineFilter>()
                .UseCommand((commandOptions) =>
                {
                    commandOptions.AddCommand<ConnectCommand>();
                    commandOptions.AddCommand<OutputCommand>();
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
                }).ConfigureServices(
                    (hostCtx, services) =>
                    {
                        services.AddSingleton<IPackageDecoder<PackageInfo>, PackageDecoder>();
                    }
                )
                .ConfigureLogging((hostCtx, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                });
        }

        static async Task Main(string[] args)
        {
            var builder = CreateSocketServerBuilder();
            builder.UseSession<ServerSession>().UseInProcSessionContainer();
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
