using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/samples")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        [HttpPost("docker")]
        public IActionResult Exec([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword)
        {
            var fileName = "docker-install.sh";
            var result = $"{rootUser}@{ip}:./{fileName}\r\n";

            var scripts = new StringBuilder();
            scripts.AppendLine("sudo yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine");
            scripts.AppendLine("sudo yum install -y yum-utils");
            scripts.AppendLine("sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo");
            scripts.AppendLine("sudo yum install -y docker-ce docker-ce-cli containerd.io");
            scripts.AppendLine("sudo systemctl start docker");
            scripts.AppendLine("docker info");

            var directory = Path.Combine("/files", ip);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileName);
            System.IO.File.WriteAllText(filePath, scripts.ToString());

            try
            {
                Cmd($"rm -rf /keys/{ip}/sshkey || true");
                Cmd($"mkdir -p /keys/{ip}/sshkey");
                Cmd($"ssh-keygen -t rsa -b 4096 -f /keys/{ip}/sshkey/id_rsa -P ''");
                Cmd($"sshpass -p {rootPassword} ssh-copy-id -o \"StrictHostKeyChecking = no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa.pub {rootUser}@{ip}");
                Cmd($"scp -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa -r \"{filePath}\" \"{rootUser}@{ip}:/root/{fileName}\"");
                Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"chmod +x /root/{fileName}\"");
                result += Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"/root/{fileName}\"");
                Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"rm -f /root/{fileName}\"");
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