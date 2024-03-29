using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class WhoCmd : ICommand
    {
        public WhoCmd()
        {
            Aliases = new[] { "who" };
            Description = "Displays current characters online.";
            Usages = new[] { "Type: who" };
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
            var sb = new StringBuilder();

            sb.Append("<ul>");

            foreach (var pc in Services.Instance.Cache.GetPlayerCache())
            {
                sb.Append($"<li>[{pc.Value.Level} {pc.Value.Race} {pc.Value.ClassName}] ");
                sb.Append($"<span class='player'>{pc.Value.Name}, {pc.Value.Title}</span></li>");
            }

            sb.Append("</ul>");
            sb.Append($"<p>Players found: {Services.Instance.Cache.GetPlayerCache().Count}</p>");

            Services.Instance.Writer.WriteLine(sb.ToString(), player);
        }
    }
}
