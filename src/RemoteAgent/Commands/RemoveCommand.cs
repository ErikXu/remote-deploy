using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Commands.Remove;

namespace RemoteAgent.Commands
{
    [Command("remove", Description = "Remove software or middleware"),
     Subcommand(typeof(RemoveDocker)), Subcommand(typeof(RemoveRabbitmq)), Subcommand(typeof(RemoveMongodb))]
    public class RemoveCommand
    {
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.Error.WriteLine("Please specify a software or middleware to remove.");
            app.ShowHelp();
            return 1;
        }
    }
}
