using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Commands.Install;

namespace RemoteAgent.Commands
{
    [Command("install", Description = "Install softwares or middlewares"),
     Subcommand(typeof(InstallDocker)), Subcommand(typeof(InstallRabbitmq)), 
     Subcommand(typeof(InstallMongodb)), Subcommand(typeof(InstallAliyunYum))]
    public class InstallCommand
    {
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.Error.WriteLine("Please specify a software or middleware to install.");
            app.ShowHelp();
            return 1;
        }
    }
}
