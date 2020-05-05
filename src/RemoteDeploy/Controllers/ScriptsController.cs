using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/scripts")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public ScriptsController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword, IFormFile file)
        {
            var result = $"{rootUser}@{ip}:./{file.FileName}\r\n";

            var directory = Path.Combine("/files", ip);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, file.FileName);
            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                var source = filePath;
                var target = $"/root/{file.FileName}";

                _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
                _commandExecutor.Scp(ip, rootUser, source, target);
                _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"chmod +x {target}");
                result +=  _commandExecutor.ExecuteCommandSSH(ip, rootUser, target);
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