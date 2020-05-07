using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;

namespace RemoteAgent
{
    public class HostedService : IHostedService
    {
        private readonly IConsole _console;
        private Timer _timer;

        public HostedService(IConsole console)
        {
            _console = console;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is started...");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}]Background service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _console.WriteLine("Background service is stopped...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}