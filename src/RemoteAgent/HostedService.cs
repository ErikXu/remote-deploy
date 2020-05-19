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
        private readonly IConfigService _configService;
        private bool _shouldReconnect = true;

        public HostedService(ILogger<HostedService> logger, IConfigService configService)
        {
            _logger = logger;

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            _configService = configService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background service is started...");

            var config = _configService.Get();

            var address = IPAddress.Parse(config.ServerIp);

            if (!await _client.ConnectAsync(new IPEndPoint(address, config.ServerPort), cancellationToken))
            {
                _logger.LogError("Failed to connect the target server.");
            }

            _client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key.ToLower())
                {
                    case "execute":
                        var command = Command.Parser.ParseFrom(Encoding.UTF8.GetBytes(package.Content));
                        await Execute(command.OperatorId, command.Content);
                        break;
                    case "connected":
                        _logger.LogInformation("Connected");
                        break;
                    default:
                        _logger.LogWarning($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Agent" + Package.Terminator));

            _client.Closed += async (sender, args) =>
            {
                if (!_shouldReconnect)
                {
                    return;
                }

                _logger.LogError("Connection disconnect.");

                Thread.Sleep(2000);

                if (!await _client.ConnectAsync(new IPEndPoint(address, config.ServerPort), cancellationToken))
                {
                    _logger.LogError("Failed to reconnect the target server.");
                }
                else
                {
                    _client.StartReceive();
                    _logger.LogInformation("Connection reconnect.");
                }
            };
        }

        private async Task Execute(string operatorId, string script)
        {
            await _client.SendAsync(Encoding.UTF8.GetBytes($"Output {operatorId} {script}{Package.Terminator}"));
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
                await _client.SendAsync(Encoding.UTF8.GetBytes($"Output {operatorId} {ex.Message}{Package.Terminator}"));
            }

            _logger.LogInformation($"OperatorId:{operatorId}, Command: [{script}] End");
        }

        private async ValueTask PrintInfo(string operatorId, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes($"Output {operatorId} {message}{Package.Terminator}"));
                _logger.LogInformation(message);
            }
        }

        private async ValueTask PrintError(string operatorId, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes($"Output {operatorId} {message}{Package.Terminator}"));
                _logger.LogError(message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _shouldReconnect = false;
            await _client.CloseAsync();
            _logger.LogInformation("Background service is stopped...");
        }
    }
}