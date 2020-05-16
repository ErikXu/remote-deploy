﻿using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteApi.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public CommandsController(IConfiguration configuration)
        {
            _serverIp = configuration["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration["RemoteServer:Port"]);
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string command)
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
            await client.SendAsync(Encoding.UTF8.GetBytes($"Execute {command}" + Package.Terminator));

            var result = string.Empty;
            while (true)
            {
                var p = await client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                if (!string.IsNullOrWhiteSpace(p.Content) && p.Content.Equals("Done", StringComparison.OrdinalIgnoreCase))
                {
                    await client.CloseAsync();
                    return Ok(result);
                }

                result += p.Content + Environment.NewLine;
            }
        }
    }
}