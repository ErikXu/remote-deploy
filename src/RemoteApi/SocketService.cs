using System;
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
        private DateTime _pongTime = DateTime.UtcNow;
        private Timer _pingTimer;
        private Timer _checkPingTimer;
        private readonly int _pingIntervalSecond = 2;
        private readonly int _checkPingIntervalSecond = 5;
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
                switch (package.Key)
                {
                    case CommandKey.Connected:
                        _logger.LogInformation("Connected");
                        break;
                    case CommandKey.Output:
                        var index = package.Content.IndexOf(' ');
                        var operatorId = package.Content.Substring(0, index);
                        var content = package.Content.Substring(index + 1, package.Content.Length - index - 1);
                        await _socketHub.Clients.Groups(operatorId).ReceiveMessage(content + Package.Terminator);
                        break;
                    case CommandKey.Pong:
                        var unix = long.Parse(package.Content);
                        _pongTime = DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;
                        break;
                    default:
                        _logger.LogError($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connect} {ClientType.Web.ToString()}{Package.Terminator}"));

            _pingTimer = new Timer(SendPing, null, TimeSpan.Zero, TimeSpan.FromSeconds(_pingIntervalSecond));
            _checkPingTimer = new Timer(CheckPong, null, TimeSpan.Zero, TimeSpan.FromSeconds(_checkPingIntervalSecond));

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

        private void SendPing(object state)
        {
            var offset = new DateTimeOffset(DateTime.UtcNow);
            _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Ping} {offset.ToUnixTimeMilliseconds()}{Package.Terminator}"));
        }

        private void CheckPong(object state)
        {
            if ((DateTime.UtcNow - _pongTime).TotalSeconds > _checkPingIntervalSecond)
            {
                if (_shouldReconnect)
                {
                    _logger.LogError("Missing pong, try to reconnect.");

                    var address = IPAddress.Parse(_serverIp);
                    if (!_client.ConnectAsync(new IPEndPoint(address, _serverPort)).Result)
                    {
                        _logger.LogError("Failed to reconnect to the target server.");
                    }
                    else
                    {
                        _client.StartReceive();

                        _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connect} {ClientType.Web.ToString()}{Package.Terminator}"));

                        _logger.LogInformation("Connection reconnect.");
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _shouldReconnect = false;
            _checkPingTimer?.Change(Timeout.Infinite, 0);
            _pingTimer?.Change(Timeout.Infinite, 0);
            await _client.CloseAsync();
            _logger.LogInformation("Socket Service is stopped...");
        }
    }
}