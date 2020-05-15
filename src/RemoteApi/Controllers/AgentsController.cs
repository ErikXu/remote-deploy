using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace RemoteApi.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IConfiguration _configuration;

        public AgentsController(ICommandExecutor commandExecutor, IConfiguration configuration)
        {
            _commandExecutor = commandExecutor;
            _configuration = configuration;
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

            var serverIp = _configuration["RemoteServer:Ip"];
            var serverPort = _configuration["RemoteServer:Port"];

            _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"mkdir -p {directory}");
            _commandExecutor.Scp(ip, rootUser, filePath, filePath);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"chmod +x {filePath}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"{filePath} config set -i {serverIp} -p {serverPort}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"nohup {filePath} -d &", true);
            _commandExecutor.RemoveSSHKey(ip, rootUser);

            return Ok();
        }
    }
}