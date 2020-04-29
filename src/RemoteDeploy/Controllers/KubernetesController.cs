using System;
using System.Diagnostics;
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
        public IActionResult ExecCommand([FromForm]string cluster, [FromForm]string command)
        {
            var result = $"{cluster}:{command}\r\n";
            var certificate = Path.Combine("/certificates", cluster, "kubeconfig");

            try
            {
                result += Cmd($"{command} --kubeconfig={certificate}");
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Ok(result);
        }

        private string Cmd(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}