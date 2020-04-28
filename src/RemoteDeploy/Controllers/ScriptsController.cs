using System;
using System.Diagnostics;
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
        [HttpPost]
        public async Task<IActionResult> Exec([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword, IFormFile file)
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
                Cmd($"rm -rf /keys/{ip}/sshkey || true");
                Cmd($"mkdir -p /keys/{ip}/sshkey");
                Cmd($"ssh-keygen -t rsa -b 4096 -f /keys/{ip}/sshkey/id_rsa -P ''");
                Cmd($"sshpass -p {rootPassword} ssh-copy-id -o \"StrictHostKeyChecking = no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa.pub {rootUser}@{ip}");
                Cmd($"scp -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa -r \"{filePath}\" \"{rootUser}@{ip}:/root/{file.FileName}\"");
                Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"chmod +x /root/{file.FileName}\"");
                result += Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"/root/{file.FileName}\"");
                Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"rm -f /root/{file.FileName}\"");
                Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"rm -f .ssh/authorized_keys\"");
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