using McMaster.Extensions.CommandLineUtils;

namespace RemoteAgent.Commands.Config
{
    [Command("get", Description = "Get current config")]
    public class GetCommand
    {
        private void OnExecute(IConsole console, IConfigService configService)
        {
            var config = configService.Get();
            if (config != null)
            {
                console.WriteLine(config.ToString());
            }
        }
    }
}