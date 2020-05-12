﻿using System;
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

        public HostedService(IConsole console)
        {
            _console = console;

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is started...");

            var ip = "127.0.0.1";
            var port = 4040;

            var address = IPAddress.Parse(ip);

            if (!await _client.ConnectAsync(new IPEndPoint(address, port), cancellationToken))
            {
                _console.WriteLine("Failed to connect the target server.");
            }

            _client.PackageHandler += async (sender, package) =>
            {
                switch (package.Key.ToLower())
                {
                    case "command":
                        await Execute(package.Content);
                        break;
                    default:
                        _console.WriteLine($"Unknown command:{package.Key}");
                        break;
                }
            };

            _client.StartReceive();

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Agent\r\n"));

        }

        private async Task Execute(string script)
        {
            await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + script + "\r\n"));

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

                process.OutputDataReceived += async (sender, args) => await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + args.Data + "\r\n"));
                process.ErrorDataReceived += async (sender, args) => await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + args.Data + "\r\n"));

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                await _client.SendAsync(Encoding.UTF8.GetBytes("Output " + ex.Message + "\r\n"));
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.CloseAsync();
            _console.WriteLine("Background service is stopped...");
        }
    }
}