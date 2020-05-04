using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Remove
{
    [Command("mongodb", Description = "Remove mongodb")]
    public class RemoveMongodb
    {
        private void OnExecute(IComandExecutor comandExecutor)
        {
            comandExecutor.Execute(Mongodb.RemoveScripts);
        }
    }
}
