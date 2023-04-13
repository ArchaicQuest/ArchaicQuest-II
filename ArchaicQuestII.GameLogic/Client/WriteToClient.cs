using System;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.SignalR;

namespace ArchaicQuestII.GameLogic.Client
{
    public class WriteToClient : IWriteToClient
    {
        private readonly TelnetHub _telnetHub;

        public WriteToClient(TelnetHub telnetHub)
        {
            _telnetHub = telnetHub;
        }

        public async void WriteLineMobSay(string mobName, string message, Player player)
        {
            try
            {
                if (player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                var mobSay = $"<span class='mob'>{mobName} says '{message}'</span>";
                await Services.Instance.Hub.Clients
                    .Client(player.ConnectionId)
                    .SendAsync("SendMessage", mobSay, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLine(string message, Player player, int delay = 0)
        {
            if (player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            try
            {
                await Task.Delay(delay);
                await Services.Instance.Hub.Clients
                    .Client(player.ConnectionId)
                    .SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void WriteLineAll(string message)
        {
            try
            {
                await Services.Instance.Hub.Clients.All.SendAsync("SendMessage", message, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void WriteToOthersInRoom(string message, Room room, Player player)
        {
            foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
            {
                WriteLine(message, pc);
            }
        }

        public void WriteToOthersInGame(string message, Player player)
        {
            var players = Services.Instance.Cache.GetAllPlayers();

            foreach (var pc in players.Where(pc => pc.Id != player.Id))
            {
                WriteLine(message, pc);
            }
        }
    }
}
