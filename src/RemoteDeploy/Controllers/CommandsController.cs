using System;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public CommandsController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        [HttpPost]
        public IActionResult Execute([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword, [FromForm]string command)
        {
            var result = $"{rootUser}@{ip}:{command}\r\n";

            try
            {
                _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
                result += _commandExecutor.ExecuteCommandSSH(ip, rootUser, command);
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