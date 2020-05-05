using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using RemoteAgent.Commands;

namespace RemoteAgent
{
    [HelpOption(Inherited = true)]
    [Command(Description = "A tool to install softwares or middlewares"),
    Subcommand(typeof(InstallCommand)), Subcommand(typeof(RemoveCommand))]
    class Program
    {
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

            var console = (IConsole)services.GetService(typeof(IConsole));

            try
            {
                return app.Execute(args);
            }
            catch (UnrecognizedCommandParsingException ex)
            {
                console.WriteLine(ex.Message);
                return -1;
            }
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("Please specify a command.");
            app.ShowHelp();
            return 1;
        }
    }
}
