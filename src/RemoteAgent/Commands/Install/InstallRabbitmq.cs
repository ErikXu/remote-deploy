using McMaster.Extensions.CommandLineUtils;

namespace RemoteAgent.Commands.Install
{
    [Command("rabbitmq", Description = "Install rabbitmq")]
    public class InstallRabbitmq
    {
        private void OnExecute(IComandExecutor comandExecutor)
        {
            comandExecutor.Execute(Installations.Rabbitmq.InstallScripts);
        }
    }
}
