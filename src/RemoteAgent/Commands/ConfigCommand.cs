using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Commands.Config;

namespace RemoteAgent.Commands
{
    [Command("config", Description = "Manage config"),
     Subcommand(typeof(GetCommand), typeof(SetCommand))]
    public class ConfigCommand
    {
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.Error.WriteLine("Please submit a sub command.");
            app.ShowHelp();
            return 1;
        }
    }
}