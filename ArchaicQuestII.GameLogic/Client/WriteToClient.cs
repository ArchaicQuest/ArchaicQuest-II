using System;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Client
{
    public class WriteToClient : IWriteToClient
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly TelnetHub _telnetHub;

        public WriteToClient(IHubContext<GameHub> hubContext, TelnetHub telnetHub)
        {
            _hubContext = hubContext;
            _telnetHub = telnetHub;
        }

        public async void WriteLine(string message, string id)
        {
            try
            {
                if (id.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                await _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLineMobSay(string mobName, string message, string id)
        {
            try
            {
                if (id.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                var mobSay = $"<span class='mob'>{mobName} says '{message}'</span>";
                await _hubContext.Clients.Client(id).SendAsync("SendMessage", mobSay, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLine(string message, string id, int delay)
        {
            try
            {
                await Task.Delay(delay);
                await _hubContext.Clients.Client(id).SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLine(string message)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void WriteLineRoom(string message, string id, int delay)
        {
            throw new NotImplementedException();
        }

        public void WriteToOthersInRoom(string message, Room room, Player player)
        {
            foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
            {
                WriteLine(message, pc.ConnectionId);
            }
        }

        public void WriteToOthersInGame(string message, Player player)
        {
            var players = Services.Instance.Cache.GetAllPlayers();

            foreach (var pc in players.Where(pc => pc.Id != player.Id))
            {
                WriteLine(message, pc.ConnectionId);
            }
        }

        public void WriteToOthersInGame(string message, string type)
        {
            var players = Services.Instance.Cache.GetAllPlayers();

            foreach (var pc in players)
            {
                WriteLine(message, pc.ConnectionId);
            }
        }
    }
}
