using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RemoteApi.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public AgentsController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
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
            _commandExecutor.RemoveSSHKey(ip, rootUser);

            return Ok();
        }
    }
}