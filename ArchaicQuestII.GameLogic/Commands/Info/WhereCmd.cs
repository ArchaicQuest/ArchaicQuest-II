using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class WhereCmd : ICommand
    {
        public WhereCmd()
        {
            Aliases = new[] { "where" };
            Description = "Displays characters whereabouts in the area.";
            Usages = new[] { "Type: where" };
            Title = "";
            DeniedStatus = null;
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
            var area = Services.Instance.Cache.GetAllRoomsInArea(room.AreaId);
            var areaName = Services.Instance.DataBase
                .GetCollection<Area>(DataBase.Collections.Area)
                .FindById(room.AreaId);

            var sb = new StringBuilder();

            sb.Append($"<p>{areaName.Title}</p><p>Players near you:</p>");
            sb.Append("<ul>");

            foreach (var rm in area)
            {
                foreach (var pc in rm.Players)
                {
                    sb.Append($"<li>{pc.Name} - {rm.Title}");
                }
            }

            sb.Append("</ul>");

            Services.Instance.Writer.WriteLine(sb.ToString(), player);
        }
    }
}
