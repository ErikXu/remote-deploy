using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Remove
{
    [Command("mongodb", Description = "Remove mongodb")]
    public class RemoveMongodb
    {
        private void OnExecute(ICommandExecutor commandExecutor)
        {
            commandExecutor.Execute(Mongodb.RemoveScripts);
        }
    }
}
