using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RemoteCommon;
using RemoteProto;
using SuperSocket.Client;

namespace RemoteAgent
{
    public class HostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IEasyClient<PackageInfo> _client;
        private DateTime _pongTime = DateTime.UtcNow;
        private Timer _pingTimer;
        private Timer _checkPingTimer;
        private readonly int _pingIntervalSecond = 2;
        private readonly int _checkPingIntervalSecond = 5;
        private bool _shouldReconnect = true;
        private readonly Config _config;

        public HostedService(ILogger<HostedService> logger, IConfigService configService)
        {
            _logger = logger;

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            _config = configService.Get();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background service is started...");

            var address = IPAddress.Parse(_config.ServerIp);

            if (!await _client.ConnectAsync(new IPEndPoint(address, _config.ServerPort), cancellationToken))
            {
                _logger.LogError("Failed to connect to the target server.");
            }

            _client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key)
                {
                    case CommandKey.Execute:
                        var command = Command.Parser.ParseFrom(Encoding.UTF8.GetBytes(package.Content));
                        await Execute(command.OperatorId, command.Content);
                        break;
                    case CommandKey.Connected:
                        _logger.LogInformation("Connected");
                        break;
                    case CommandKey.Pong:
                        var unix = long.Parse(package.Content);
                        _pongTime = DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;
                        break;
                    default:
                        _logger.LogWarning($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connect} {ClientType.Agent.ToString()}{Package.Terminator}"));

            _pingTimer = new Timer(SendPing, null, TimeSpan.Zero, TimeSpan.FromSeconds(_pingIntervalSecond));
            _checkPingTimer = new Timer(CheckPong, null, TimeSpan.Zero, TimeSpan.FromSeconds(_checkPingIntervalSecond));

            _client.Closed += async (sender, args) =>
            {
                if (!_shouldReconnect)
                {
                    return;
                }

                _logger.LogError("Connection disconnect.");

                Thread.Sleep(2000);

                if (!await _client.ConnectAsync(new IPEndPoint(address, _config.ServerPort), cancellationToken))
                {
                    _logger.LogError("Failed to reconnect to the target server.");
                }
                else
                {
                    _client.StartReceive();
                    _logger.LogInformation("Connection reconnect.");
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

                    var address = IPAddress.Parse(_config.ServerIp);
                    if (!_client.ConnectAsync(new IPEndPoint(address, _config.ServerPort)).Result)
                    {
                        _logger.LogError("Failed to reconnect to the target server.");
                    }
                    else
                    {
                        _client.StartReceive();

                        _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connect} {ClientType.Agent.ToString()}{Package.Terminator}"));

                        _logger.LogInformation("Connection reconnect.");
                    }
                }
            }
        }

        private async Task Execute(string operatorId, string script)
        {
            await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Output} {operatorId} {script}{Package.Terminator}"));
            _logger.LogInformation($"OperatorId:{operatorId}, Command: [{script}] Start");

            try
            {
                var escapedArgs = script.Replace("\"", "\\\"");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.OutputDataReceived += async (sender, args) => await PrintInfo(operatorId, args.Data);
                process.ErrorDataReceived += async (sender, args) => await PrintError(operatorId, args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Output} {operatorId} {ex.Message}{Package.Terminator}"));
            }

            _logger.LogInformation($"OperatorId:{operatorId}, Command: [{script}] End");
        }

        private async ValueTask PrintInfo(string operatorId, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Output} {operatorId} {message}{Package.Terminator}"));
                _logger.LogInformation(message);
            }
        }

        private async ValueTask PrintError(string operatorId, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Output} {operatorId} {message}{Package.Terminator}"));
                _logger.LogError(message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _shouldReconnect = false;
            _checkPingTimer?.Change(Timeout.Infinite, 0);
            _pingTimer?.Change(Timeout.Infinite, 0);
            await _client.CloseAsync();
            _logger.LogInformation("Background service is stopped...");
        }
    }
}