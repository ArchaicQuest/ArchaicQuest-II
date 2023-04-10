using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.World
{
    public class ScanCmd : ICommand
    {
        public ScanCmd()
        {
            Aliases = new[] { "scan" };
            Description = "Scan the current area or a certain direction.";
            Usages = new[] { "Example: scan", "Example: scan north" };
            Title = "";
            DeniedStatus = new[]
            {
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Fighting,
                CharacterStatus.Status.Ghost,
                CharacterStatus.Status.Fleeing,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Stunned,
            };
            UserRole = UserRole.Player;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);

            if (!string.IsNullOrEmpty(target))
            {
                ScanDirection(player, room, target);
                return;
            }

            var sb = new StringBuilder();

            sb.Append("<span>Right here:</span>");

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

                var getRoomObj = Services.Instance.Cache.GetRoom(
                    $"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}"
                );

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

                if (
                    getRoomObj.Mobs.All(x => x.IsHiddenScriptMob == false)
                    && !getRoomObj.Players.Any()
                )
                {
                    sb.Append("<p>There is nobody there.</p>");
                }
            }

            Services.Instance.Writer.WriteLine(sb.ToString(), player.ConnectionId);
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

            var getDirection = directions.FirstOrDefault(
                x => x.StartsWith(direction, StringComparison.CurrentCultureIgnoreCase)
            );

            if (getDirection == null)
            {
                Services.Instance.Writer.WriteLine(
                    "You can't look in that direction.",
                    player.ConnectionId
                );
                return;
            }

            var getRoomCoords = Helpers.IsExit(getDirection, room);

            var getRoomObj = Services.Instance.Cache.GetRoom(
                $"{getRoomCoords.AreaId}{getRoomCoords.Coords.X}{getRoomCoords.Coords.Y}{getRoomCoords.Coords.Z}"
            );
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

            Services.Instance.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}
