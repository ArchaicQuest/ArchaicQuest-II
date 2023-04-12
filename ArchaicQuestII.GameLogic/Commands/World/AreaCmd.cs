using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.World
{
    public class AreaCmd : ICommand
    {
        public AreaCmd()
        {
            Aliases = new[] { "area" };
            Description = "Displays info about area.";
            Usages = new[] { "Type: area" };
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
            var sb = new StringBuilder();
            var area = room.GetArea();
            var roomCount = Services.Instance.Cache.GetAllRoomsInArea(room.AreaId).Count;

            if (string.IsNullOrEmpty(target))
            {
                sb.Append(
                    $"<p>You are currently in <b>{area.Title}</b>.</p><p>{area.Description}</p>"
                );
                sb.Append($"<p>{AreaPopulation(room)}<.p>");
                sb.Append($"<p>{AreaConsider(player, room)}<.p>");

                sb.Append(
                    roomCount > 1
                        ? $"<p>Area contains <b>{roomCount}</b> rooms.</p>"
                        : "<p>Area contains <b>1</b> room.</p>"
                );

                if (area.CreatedBy != null)
                    sb.Append($"<p>(Created by {area.CreatedBy})</p>");
            }

            if (target == "list")
            {
                var areas = Services.Instance.DataBase
                    .GetCollection<Area>(DataBase.Collections.Area)
                    .FindAll()
                    .ToList();

                sb.Append($"Total Areas: {areas.Count}");
                sb.Append("<ul>");

                foreach (var a in areas)
                {
                    sb.Append($"<li>[{GetAreaLevelScale(a)}] {a.Title}");
                    if (a.CreatedBy != null)
                        sb.Append($" ({a.CreatedBy})");
                    sb.Append("</li>");
                }

                sb.Append("</ul>");
            }

            if (target is "consider" or "con")
            {
                Services.Instance.Writer.WriteLine(AreaConsider(player, room), player);
                return;
            }

            if (target is "pop" or "population")
            {
                Services.Instance.Writer.WriteLine(AreaPopulation(room), player);
                return;
            }

            Services.Instance.Writer.WriteLine(sb.ToString(), player);
        }

        /// <summary>
        /// Display player difficulty for area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        private string AreaConsider(Player player, Room room)
        {
            var mobLevels = 0;
            var mobCount = 0;

            foreach (
                var mob in Services.Instance.Cache
                    .GetAllRoomsInArea(room.AreaId)
                    .SelectMany(r => r.Mobs)
            )
            {
                mobLevels += mob.Level;
                mobCount++;
            }

            var dangerLevel = mobCount == 0 ? 0 : mobLevels / mobCount - player.Level;

            return dangerLevel switch
            {
                > 10 => "{red}You feel nervous here!{/}",
                > 5 => "{yellow}You feel anxious here.{/}",
                > 1 => "{blue}You feel comfortable here.{/}",
                _ => "{green}You feel relaxed here.{/}"
            };
        }

        /// <summary>
        /// Display player population in area
        /// </summary>
        /// <param name="room">Room where command was entered</param>
        private string AreaPopulation(Room room)
        {
            var playerCount = Services.Instance.Cache
                .GetAllRoomsInArea(room.AreaId)
                .SelectMany(r => r.Players)
                .Count();

            return playerCount switch
            {
                > 30 => "The area shows signs of being heavily traveled.",
                > 20 => "The area shows signs of being well traveled.",
                > 10 => "The area shows signs of being traveled.",
                > 1 => "The area shows signs of being lightly traveled.",
                _ => "The area shows no signs of being traveled."
            };
        }

        /// <summary>
        /// Helper to get area levels
        /// </summary>
        /// <param name="area">Area to get level scale</param>
        private string GetAreaLevelScale(Area area)
        {
            var minLvl = 999;
            var maxLvl = 0;
            var mobCount = 0;

            foreach (var mob in area.Rooms.Where(x => x.Mobs.Any()).SelectMany(room => room.Mobs))
            {
                if (mob.Level < minLvl)
                    minLvl = mob.Level;
                if (mob.Level > maxLvl)
                    maxLvl = mob.Level;

                mobCount++;
            }

            return mobCount == 0 ? "0 - 0" : $"{minLvl} - {maxLvl}";
        }
    }
}
