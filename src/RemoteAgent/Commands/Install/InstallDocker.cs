using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Install
{
    [Command("docker", Description = "Install docker")]
    public class InstallDocker
    {
        private void OnExecute(ICommandExecutor commandExecutor)
        {
            commandExecutor.Execute(Docker.InstallScripts);
        }
    }
}
