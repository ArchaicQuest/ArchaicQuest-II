
using System;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.SignalR;
using ArchaicQuestII.Core.Events;
using System.IO;

namespace ArchaicQuestII.Hubs
{
    public class GameHub : Hub
    {
        private Log.Log _logger { get; set; }
       
        public GameHub()
        {
            _logger = new Log.Log();
            
        }
        /// <summary>
        /// Do action when user connects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
           // await Clients.All.SendAsync("SendAction", "user", "joined");
        }

        /// <summary>
        /// Do action when user disconnects 
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendAction", "user", "left");
        }

        /// <summary>
        /// Send message to all clients
        /// </summary>
        /// <returns></returns>
        public async Task Send(string message)
        {
            _logger.Information($"Player sent {message}");
            await Clients.All.SendAsync("SendMessage", "user x", message);
        }

        /// <summary>
        /// Send message to specific client
        /// </summary>
        /// <returns></returns>
        public async Task SendToClient(string message, string hubId)
        {
            _logger.Information($"Player sent {message}");
            await Clients.Client(hubId).SendAsync("SendMessage", message);
        }

        public async void Welcome(string id)
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);

            var motd = File.ReadAllText(directory + "/motd");  

           await SendToClient(motd, id);
        }

        public void CreateCharacter(string name = "Liam")
        {
            var newPlayer = new Player()
            {
                Name = name
            };

            DB.Save(newPlayer, "Player");

        }
    }
}
