using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class ScanCmd : ICommand
    {
        public ScanCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"scan"};
            Description = "Scan the current area or a certain direction.";
            Usages = new[] {"Example: scan", "Example: scan north"};
            UserRole = UserRole.Player;
            Writer = writeToClient;
            Cache = cache;
            UpdateClient = updateClient;
            RoomActions = roomActions;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public IWriteToClient Writer { get; }
        public ICache Cache { get; }
        public IUpdateClientUI UpdateClient { get; }
        public IRoomActions RoomActions { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);
            
            if (!string.IsNullOrEmpty(target))
            {
                ScanDirection(player, room, target);
                return;
            }

            var sb = new StringBuilder();

            sb.Append($"<span>Right here:</span>");

            foreach (var obj in room.Mobs.Where(x => x.IsHiddenScriptMob == false))
            {
                sb.Append($"<p class='mob'>{obj.Name} is right here.</p>");
            }

            foreach (var obj in room.Players)
            {
                sb.Append($"<p class='player'>{obj.Name} is right here.</p>");
            }

            if (room.Mobs.All(x => x.IsHiddenScriptMob == false) && !room.Players.Any())
            {
                sb.Append("<p>There is nobody here.</p>");
            }

            foreach (var exit in Helpers.GetListOfExits(room.Exits))
            {
                var getRoomCoords = Helpers.IsExit(exit, room);

                var getRoomObj =
                    Cache.GetRoom(
                        $"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}");

                sb.Append($"<span>{exit}:</span>");

                foreach (var obj in getRoomObj.Mobs.Where(x => x.IsHiddenScriptMob == false))
                {
                    if (exit.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is below you.</p>");
                    }
                    else if (exit.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is above you.</p>");
                    }
                    else
                    {
                        sb.Append($"<p class='mob'>{obj.Name} is to the {exit}.</p>");
                    }
                }

                foreach (var obj in getRoomObj.Players)
                {
                    if (exit.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='player'>{obj.Name} is below you.</p>");
                    }
                    else if (exit.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sb.Append($"<p class='player'>{obj.Name} is above you.</p>");
                    }
                    else
                    {
                        sb.Append($"<p class='player'>{obj.Name} is to the {exit}.</p>");
                    }
                }

                if (getRoomObj.Mobs.All(x => x.IsHiddenScriptMob == false) && !getRoomObj.Players.Any())
                {
                    sb.Append("<p>There is nobody there.</p>");
                }
            }

            Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }

        private void ScanDirection(Player player, Room room, string direction)
        {
            var directions = new List<string>
            {
               "North",
               "East",
               "South",
               "West",
               "North West",
               "North East",
               "South East",
               "South West",
               "Up",
               "Down"
           };

            var getDirection = directions.FirstOrDefault(x => x.StartsWith(direction, StringComparison.CurrentCultureIgnoreCase));

            if (getDirection == null)
            {
                Writer.WriteLine("You can't look in that direction.", player.ConnectionId);
            }

            var getRoomCoords = Helpers.IsExit(getDirection, room);

            var getRoomObj = Cache.GetRoom($"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}");
            var sb = new StringBuilder();

            sb.Append($"<span>You peer intently {getDirection}</span>");

            foreach (var obj in getRoomObj.Mobs)
            {
                if (getDirection.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='mob'>{obj.Name} is below you.</p>");
                }
                else if (getDirection.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='mob'>{obj.Name} is above you.</p>");
                }
                else
                {
                    sb.Append($"<p class='mob'>{obj.Name} is to the {getDirection}.</p>");
                }
            }

            foreach (var obj in getRoomObj.Players)
            {
                if (getDirection.Equals("down", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='player'>{obj.Name} is below you.</p>");
                }
                else if (getDirection.Equals("up", StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"<p class='player'>{obj.Name} is above you.</p>");
                }
                else
                {
                    sb.Append($"<p class='player'>{obj.Name} is to the {getDirection}.</p>");
                }
            }

            if (!getRoomObj.Mobs.Any() && !getRoomObj.Players.Any())
            {
                sb.Append("<p>There is nobody there.</p>");
            }

            Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}