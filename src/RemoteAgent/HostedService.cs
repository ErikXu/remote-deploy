using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteAgent
{
    public class HostedService : IHostedService
    {
        private readonly IConsole _console;
        private readonly IEasyClient<PackageInfo> _client;
        private readonly IConfigService _configService;

        public HostedService(IConsole console, IConfigService configService)
        {
            _console = console;

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            _configService = configService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is started...");

            var config = _configService.Get();

            var address = IPAddress.Parse(config.ServerIp);

            if (!await _client.ConnectAsync(new IPEndPoint(address, config.ServerPort), cancellationToken))
            {
                _console.WriteLine("Failed to connect the target server.");
            }

            _client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key.ToLower())
                {
                    case "execute":
                        await Execute(package.Content);
                        break;
                    case "connected":
                        _console.WriteLine("Connected");
                        break;
                    default:
                        _console.WriteLine($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Agent" + Package.Terminator));

        }

        private async Task Execute(string script)
        {
            await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + script + Package.Terminator));

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

                process.OutputDataReceived += async (sender, args) => await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + args.Data + Package.Terminator));
                process.ErrorDataReceived += async (sender, args) => await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + args.Data + Package.Terminator));

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + ex.Message + Package.Terminator));
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.CloseAsync();
            _console.WriteLine("Background service is stopped...");
        }
    }
}