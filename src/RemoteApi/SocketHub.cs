﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace RemoteApi
{
    public interface ISocketClient
    {
        Task ReceiveMessage(string message);
    }

    public class SocketHub : Hub<ISocketClient>
    {
        public async Task Subscribe(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task SendToGroup(string group, string message)
        {
            await Clients.Groups(group).ReceiveMessage(message);
        }
    }
}