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
        private readonly IEasyClient<PackageInfo> _client;
        private bool _shouldReconnect = true;

        public SocketService(IConfiguration configuration, ILogger<SocketService> logger, IHubContext<SocketHub, ISocketClient> socketHub)
        {
            _serverIp = configuration["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration["RemoteServer:Port"]);

            _socketHub = socketHub;
            _logger = logger;

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Socket Service is starting.");

            var address = IPAddress.Parse(_serverIp);

            if (!await _client.ConnectAsync(new IPEndPoint(address, _serverPort), cancellationToken))
            {
                _logger.LogError("Failed to connect to the socket server.");
            }

            _client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key.ToLower())
                {
                    case "connected":
                        _logger.LogInformation("Connected");
                        break;
                    case "output":
                        var index = package.Content.IndexOf(' ');
                        var operatorId = package.Content.Substring(0, index);
                        var content = package.Content.Substring(index + 1, package.Content.Length - index - 1);
                        await _socketHub.Clients.Groups(operatorId).ReceiveMessage(content + Package.Terminator);
                        break;
                    default:
                        _logger.LogError($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Web" + Package.Terminator));

            _client.Closed += async (sender, args) =>
            {
                if (!_shouldReconnect)
                {
                    return;
                }

                Thread.Sleep(2000);

                if (!await _client.ConnectAsync(new IPEndPoint(address, _serverPort), cancellationToken))
                {
                    _logger.LogError("Failed to connect to the socket server.");
                }
                else
                {
                    _client.StartReceive();
                }
            };
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _shouldReconnect = false;
            await _client.CloseAsync();
            _logger.LogInformation("Socket Service is stopped...");
        }
    }
}