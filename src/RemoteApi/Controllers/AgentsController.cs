﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteApi.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;

        private readonly string _serverIp;
        private readonly int _serverPort;

        public AgentsController(ICommandExecutor commandExecutor, IConfiguration configuration)
        {
            _commandExecutor = commandExecutor;

            _serverIp = configuration["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration["RemoteServer:Port"]);
        }

        /// <summary>
        /// List all agents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            var client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
            var address = IPAddress.Parse(_serverIp);

            if (!await client.ConnectAsync(new IPEndPoint(address, _serverPort)))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to connect to the server.");
            }

            await client.SendAsync(Encoding.UTF8.GetBytes("Connect Web" + Package.Terminator));

            while (true)
            {
                var p = await client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                switch (p.Key.ToLower())
                {
                    case "connected":
                        await client.SendAsync(Encoding.UTF8.GetBytes("ListAgent" + Package.Terminator));
                        break;
                    default:
                        var agents = JsonConvert.DeserializeObject<List<AgentInfo>>(p.Content);
                        await client.CloseAsync();
                        return Ok(agents);
                }
            }
        }

        /// <summary>
        /// upload the latest agent
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Install agent to specified machine
        /// </summary>
        /// <returns></returns>
        [HttpPost("install")]
        public IActionResult Execute([FromForm]string ip, [FromForm]string rootUser, [FromForm]string rootPassword)
        {
            var directory = "/agents";
            var filePath = Path.Combine(directory, "remote-agent");

            _commandExecutor.AddSSHKey(ip, rootUser, rootPassword);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"mkdir -p {directory}");
            _commandExecutor.Scp(ip, rootUser, filePath, filePath);
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"chmod +x {filePath}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"{filePath} config set -i {_serverIp} -p {_serverPort}");
            _commandExecutor.ExecuteCommandSSH(ip, rootUser, $"nohup {filePath} -d &", true);
            _commandExecutor.RemoveSSHKey(ip, rootUser);

            return Ok();
        }
    }
}