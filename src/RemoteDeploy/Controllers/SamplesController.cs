using System;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/samples")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IShellGenerator _shellGenerator;

        public SamplesController(ICommandExecutor commandExecutor, IShellGenerator shellGenerator)
        {
            _commandExecutor = commandExecutor;
            _shellGenerator = shellGenerator;
        }

        [HttpPost("docker")]
        public IActionResult InstallDocker([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword)
        {
            var shellResult = _shellGenerator.GenerateDockerShell(ip);
            return ExecuteShell(ip, rootUser, rootPassword, shellResult);
        }

        [HttpPost("rabbit")]
        public IActionResult InstallRabbitMq([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword, [FromForm]string rabbitUser, [FromForm]string rabbitPassword, [FromForm]string vhost = "/")
        {
            var shellResult = _shellGenerator.GenerateRabbitShell(ip, rabbitUser, rabbitPassword, vhost);
            return ExecuteShell(ip, rootUser, rootPassword, shellResult);
        }

        private IActionResult ExecuteShell(string ip, string rootUser, string rootPassword, GeneratorResult shellResult)
        {
            var result = $"{rootUser}@{ip}:./{shellResult.FileName}\r\n";

            try
            {
                var source = shellResult.FilePath;
                var target = $"/root/{shellResult.FileName}";
                _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
                _commandExecutor.Scp(ip, rootUser, source, target);
                _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"chmod +x {target}");
                result += _commandExecutor.ExecuteCommandSSH(ip, rootUser, target);
                _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"rm -f {target}");
                _commandExecutor.RemoveSSHKey(ip, rootUser);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Ok(result);
        }
    }
}