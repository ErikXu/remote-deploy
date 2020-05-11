using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;

namespace RemoteAgent
{
    public class HostedService : IHostedService
    {
        private readonly IConsole _console;
        private readonly Socket _socket;
        private readonly ICommandExecutor _commandExecutor;

        public HostedService(IConsole console, ICommandExecutor commandExecutor)
        {
            _console = console;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _commandExecutor = commandExecutor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is started...");

            var ip = "127.0.0.1";
            var port = 4040;

            var address = IPAddress.Parse(ip);
            var endPoint = new IPEndPoint(address, port);

            _socket.Connect(endPoint);

            var res = new byte[1024];

            Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     var length = _socket.Receive(res, res.Length, SocketFlags.None);

                     var packages = Encoding.UTF8.GetString(res, 0, length).Split(' ', 2);
                     var command = packages[0];
                     var content = string.Empty;
                     if (packages.Length == 2)
                     {
                         content = packages[1];
                     }

                     if (command.Equals("Disconnect", StringComparison.OrdinalIgnoreCase))
                     {
                         return;
                     }

                     switch (command.ToLower())
                     {
                         case "command":
                             Execute(content);
                             break;
                         default:
                             _console.WriteLine($"Unknown command:{command}");
                             break;
                     }
                 }
             }, cancellationToken);

            _socket.Send(Encoding.UTF8.GetBytes("Connect\r\n"));

            return Task.CompletedTask;
        }

        private void Execute(string script)
        {
            _socket.Send(Encoding.UTF8.GetBytes("Output " + script + "\r\n"));

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

                process.OutputDataReceived += (sender, args) => _socket.Send(Encoding.UTF8.GetBytes("Output " + args.Data + "\r\n"));
                process.ErrorDataReceived += (sender, args) => _socket.Send(Encoding.UTF8.GetBytes("Output " + args.Data + "\r\n"));

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _socket.Send(Encoding.UTF8.GetBytes("Output " + ex.Message + "\r\n"));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _socket.Disconnect(false);
            _console.WriteLine("Background service is stopped...");
            return Task.CompletedTask;
        }
    }
}