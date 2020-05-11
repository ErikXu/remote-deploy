using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteAgent.Commands;

namespace RemoteAgent
{
    [HelpOption(Inherited = true)]
    [Command(Description = "A tool to install software or middleware"),
    Subcommand(typeof(InstallCommand)), Subcommand(typeof(RemoveCommand))]
    class Program
    {
        private readonly IConsole _console;

        [Option("-d|--detach", description: "Run in background", CommandOptionType.NoValue)]
        public bool Detach { get; set; }

        public Program(IConsole console)
        {
            _console = console;
        }

        public static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(PhysicalConsole.Singleton);
            serviceCollection.AddSingleton<ICommandExecutor, CommandExecutor>();

            var services = serviceCollection.BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            try
            {
                return app.Execute(args);
            }
            catch (UnrecognizedCommandParsingException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        private int OnExecute(CommandLineApplication app)
        {
            if (Detach)
            {
                var builder = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddSingleton(PhysicalConsole.Singleton);
                        services.AddHostedService<HostedService>();
                        services.AddSingleton<ICommandExecutor, CommandExecutor>();
                    }).UseConsoleLifetime()
                    .Build();

                builder.Run();
            }
            else
            {
                _console.WriteLine("Please specify a command.");
                app.ShowHelp();
            }

            return 1;
        }
    }
}
