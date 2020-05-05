using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Remove
{
    [Command("docker", Description = "Remove docker")]
    public class RemoveDocker
    {
        private void OnExecute(ICommandExecutor commandExecutor)
        {
            commandExecutor.Execute(Docker.RemoveScripts);
        }
    }
}
