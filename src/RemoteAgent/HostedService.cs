using System;
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

        public HostedService(IConsole console)
        {
            _console = console;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is started...");
            _socket.Connect("127.0.0.1", 4040);
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}]Background service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is stopped...");
            return Task.CompletedTask;
        }
    }
}