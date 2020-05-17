using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteApi
{
    public class SocketService : IHostedService
    {
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly IHubContext<SocketHub, ISocketClient> _socketHub;
        private readonly ILogger _logger;

        public SocketService(IConfiguration configuration, ILogger<SocketService> logger, IHubContext<SocketHub, ISocketClient> socketHub)
        {
            _serverIp = configuration["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration["RemoteServer:Port"]);

            _socketHub = socketHub;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Socket Service is starting.");

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            var client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            var address = IPAddress.Parse(_serverIp);

            if (!await client.ConnectAsync(new IPEndPoint(address, _serverPort), cancellationToken))
            {
                _logger.LogError("Failed to connect to the socket server.");
            }

            client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key.ToLower())
                {
                    case "connected":
                        _logger.LogInformation("Connected");
                        break;
                    case "output":
                        var temp = package.Content.Split(' ');
                        await _socketHub.Clients.Groups(temp[0]).ReceiveMessage(temp[1] + Package.Terminator);
                        break;
                    default:
                        _logger.LogError($"Unknown command:{package.Key}");
                        break;
                }
            };

            client.StartReceive();

            await client.SendAsync(Encoding.UTF8.GetBytes("Connect Web" + Package.Terminator));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Socket Service is stopping.");

            return Task.CompletedTask;
        }
    }
}