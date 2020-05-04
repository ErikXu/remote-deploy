using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Install
{
    [Command("mongodb", Description = "Install mongodb")]
    public class InstallMongodb
    {
        private void OnExecute(IComandExecutor comandExecutor)
        {
            comandExecutor.Execute(Mongodb.InstallScripts);
        }
    }
}
