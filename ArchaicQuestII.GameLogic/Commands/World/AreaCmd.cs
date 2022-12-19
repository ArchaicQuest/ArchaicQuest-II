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
        public AreaCmd(ICore core)
        {
            Aliases = new[] {"area"};
            Description = "Displays info about area.";
            Usages = new[] {"Type: area"};
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
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var target = input.ElementAtOrDefault(1);
            var sb = new StringBuilder();
            var area = Core.RoomActions.GetRoomArea(room);
            var roomCount = Core.Cache.GetAllRoomsInArea(room.AreaId).Count;
            
            if (string.IsNullOrEmpty(target))
            {
                sb.Append($"<p>You are currently in <b>{area.Title}</b>.</p><p>{area.Description}</p>");
                sb.Append($"<p>{Core.AreaActions.AreaPopulation(player, room)}<.p>");
                sb.Append($"<p>{Core.AreaActions.AreaConsider(player, room)}<.p>");

                sb.Append(roomCount > 1
                    ? $"<p>Area contains <b>{roomCount}</b> rooms.</p>"
                    : "<p>Area contains <b>1</b> room.</p>");

                if (area.CreatedBy != null)
                    sb.Append($"<p>(Created by {area.CreatedBy})</p>");
            }

            if (target == "list")
            {
                var areas = Core.DataBase.GetCollection<Area>(DataBase.Collections.Area).FindAll().ToList();

                sb.Append($"Total Areas: {areas.Count}");
                sb.Append("<ul>");
           
                foreach (var a in areas)
                {
                    sb.Append($"<li>[{Core.AreaActions.GetAreaLevelScale(a)}] {a.Title}");
                    if (a.CreatedBy != null)
                        sb.Append($" ({a.CreatedBy})");
                    sb.Append("</li>");
                }

                sb.Append("</ul>");
            }

            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}