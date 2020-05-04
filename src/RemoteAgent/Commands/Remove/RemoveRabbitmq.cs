using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Remove
{
    [Command("rabbitmq", Description = "Remove rabbitmq")]
    public class RemoveRabbitmq
    {
        private void OnExecute(IComandExecutor comandExecutor)
        {
            comandExecutor.Execute(Rabbitmq.RemoveScripts);
        }
    }
}
