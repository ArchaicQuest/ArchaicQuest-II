
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Core.Player;
using Serilog;

namespace ArchaicQuestII.Hubs
{
    public class GameHub : Hub
    {
        private Log.Log _logger { get; set; }
        private DB _save { get; set; }

        public GameHub()
        {
            _logger = new Log.Log();
            _save = new DB();
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("SendAction", "user", "joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendAction", "user", "left");
        }

        public async Task Send(string message)
        {
            _logger.Information($"Player sent {message}");
            await Clients.All.SendAsync("SendMessage", "user", message);
        }

        public async Task SendToClient(string message, string hubId)
        {
            _logger.Information($"Player sent {message}");
            await Clients.Client(hubId).SendAsync("SendMessage", "user", message);
        }

        public void CreateCharacter(string name = "Liam")
        {
            var newPlayer = new Player()
            {
                Name = name
            };

            _save.SavePlayer(newPlayer);

        }
    }
}
