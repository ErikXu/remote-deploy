using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RemoteCommon;
using SuperSocket.Client;

namespace RemoteApi.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly int _serverPort;

        private readonly IEasyClient<PackageInfo> _client;
        private readonly IPAddress _address;

        public ClientsController(IConfiguration configuration)
        {
            var serverIp = configuration["RemoteServer:Ip"];
            _serverPort = int.Parse(configuration["RemoteServer:Port"]);
            _address = IPAddress.Parse(serverIp);

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new PackageDecoder()
            };

            _client = new EasyClient<PackageInfo>(pipelineFilter).AsClient();
        }

        /// <summary>
        /// List all clients
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            if (!await _client.ConnectAsync(new IPEndPoint(_address, _serverPort)))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to connect to the server.");
            }

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Web" + Package.Terminator));

            while (true)
            {
                var p = await _client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                switch (p.Key.ToLower())
                {
                    case "connected":
                        await _client.SendAsync(Encoding.UTF8.GetBytes("ListClient" + Package.Terminator));
                        break;
                    default:
                        var clients = JsonConvert.DeserializeObject<List<ClientInfo>>(p.Content).OrderByDescending(n => n.ConnectTime);
                        await _client.CloseAsync();
                        return Ok(clients);
                }
            }
        }

        /// <summary>
        /// Delete client
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _client.ConnectAsync(new IPEndPoint(_address, _serverPort)))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to connect to the server.");
            }

            await _client.SendAsync(Encoding.UTF8.GetBytes("Connect Web" + Package.Terminator));

            while (true)
            {
                var p = await _client.ReceiveAsync();

                if (p == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Connection dropped.");
                }

                switch (p.Key.ToLower())
                {
                    case "connected":
                        await _client.SendAsync(Encoding.UTF8.GetBytes($"Disconnect {id}" + Package.Terminator));
                        break;
                    default:
                        await _client.CloseAsync();
                        return Ok(p.Content);
                }
            }
        }
    }
}