using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Install
{
    [Command("aliyun-yum", Description = "Remove aliyun yum")]
    public class InstallAliyunYum
    {
        private void OnExecute(ICommandExecutor commandExecutor)
        {
            commandExecutor.Execute(AliyunYum.Scripts);
        }
    }
}
