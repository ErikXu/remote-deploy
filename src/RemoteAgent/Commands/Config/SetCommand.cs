using McMaster.Extensions.CommandLineUtils;

namespace RemoteAgent.Commands.Config
{
    [Command("set", Description = "Set config")]
    public class SetCommand
    {
        [Option("-i|--ip")]
        public string Ip { get; set; }

        [Option("-p|--port")]
        public int Port { get; set; }

        private void OnExecute(IConfigService configService)
        {
            configService.Set(Ip, Port);
        }
    }
}