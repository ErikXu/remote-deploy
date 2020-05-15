using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteApi.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        private readonly string _serverIp;
        private readonly int _serverPort;

        public AgentsController(ICommandExecutor commandExecutor, IConfiguration configuration)
        {
            _commandExecutor = commandExecutor;

            var configuration1 = configuration;
            _serverIp = configuration1["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration1["RemoteServer:Port"]);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            var client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            var address = IPAddress.Parse(_serverIp);

            if (!await client.ConnectAsync(new IPEndPoint(address, _serverPort)))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to connect to the server.");
            }

            await client.SendAsync(Encoding.UTF8.GetBytes("ListAgent" + Package.Terminator));

            while (true)
            {
                var p = await client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                var agents = JsonConvert.DeserializeObject<List<AgentInfo>>(p.Content);
                return Ok(agents);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var directory = "/agents";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, "remote-agent");
            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        [HttpPost("install")]
        public IActionResult Execute([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword)
        {
            var directory = "/agents";
            var filePath = Path.Combine(directory, "remote-agent");

            _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"mkdir -p {directory}");
            _commandExecutor.Scp(ip, rootUser, filePath, filePath);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"chmod +x {filePath}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"{filePath} config set -i {_serverIp} -p {_serverPort}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"nohup {filePath} -d &", true);
            _commandExecutor.RemoveSSHKey(ip, rootUser);

            return Ok();
        }
    }
}