using McMaster.Extensions.CommandLineUtils;
using RemoteAgent.Installations;

namespace RemoteAgent.Commands.Install
{
    [Command("aliyun-yum", Description = "Remove aliyun yum")]
    public class InstallAliyunYum
    {
        private void OnExecute(IComandExecutor comandExecutor)
        {
            comandExecutor.Execute(AliyunYum.Scripts);
        }
    }
}
