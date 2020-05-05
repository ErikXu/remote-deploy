using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/kubernetes")]
    [ApiController]
    public class KubernetesController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        public KubernetesController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        /// <summary>
        /// Upload kubeconfig
        /// </summary>
        [HttpPost("certificates")]
        public async Task<IActionResult> UploadCertificate([FromForm]string cluster, IFormFile file)
        {
            var directory = Path.Combine("/certificates", cluster);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var certificatePath = Path.Combine(directory, "kubeconfig");
            await using (var stream = System.IO.File.Create(certificatePath))
            {
                await file.CopyToAsync(stream);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ExecuteCommand([FromForm]string cluster, [FromForm]string command)
        {
            var result = $"{cluster}:{command}\r\n";
            var certificate = Path.Combine("/certificates", cluster, "kubeconfig");

            try
            {
                result += _commandExecutor.ExecuteCommand($"{command} --kubeconfig={certificate}");
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Ok(result);
        }
    }
}