using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RemoteApi.Models;
using RemoteCommon;
using RemoteProto;
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
        public async Task<IActionResult> Execute(CommandForm form)
        {
            var command = new Command
            {
                OperatorId = form.OperatorId,
                Ip = form.Ip,
                Content = form.Command
            };

            string content;

            await using (var stream = new MemoryStream())
            {
                command.WriteTo(stream);
                content = Encoding.UTF8.GetString(stream.ToArray());
            }
            
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

            await client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Connect} {ClientType.Short.ToString()}{Package.Terminator}"));
            await client.SendAsync(Encoding.UTF8.GetBytes($"{CommandKey.Execute} {content}{Package.Terminator}"));

            while (true)
            {
                var p = await client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                if (!string.IsNullOrWhiteSpace(p.Content) && p.Content.Equals("Started", StringComparison.OrdinalIgnoreCase))
                {
                    await client.CloseAsync();
                    return Ok(form.OperatorId);
                }
            }
        }
    }
}