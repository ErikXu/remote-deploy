using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RemoteDeploy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Exec(string ip, string rootUser, string rootPassword)
        {
            Cmd("rm -rf ./deploy/sshkey || true");
            Cmd("mkdir -p ./deploy/sshkey");
            Cmd("ssh-keygen -t rsa -b 4096 -f ./deploy/sshkey/id_rsa -P ''");
            Cmd($"sshpass -p {rootPassword} ssh-copy-id -o \"StrictHostKeyChecking = no\" -o \"UserKnownHostsFile=/dev/null\" -i ./deploy/sshkey/id_rsa.pub {rootUser}@{ip}");
            Cmd($"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i ./deploy/sshkey/id_rsa \"{rootUser}@{ip}\" \"ls /root\"");
            return Ok();
        }

        private static string Cmd(string cmd)
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