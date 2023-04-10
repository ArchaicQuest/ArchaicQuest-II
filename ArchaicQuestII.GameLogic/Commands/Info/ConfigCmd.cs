using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class ConfigCmd : ICommand
    {
        public ConfigCmd()
        {
            Aliases = new[] { "config" };
            Description = "Displays current config settings. Use the UI Cog to edit your settings.";
            Usages = new[] { "Type: config" };
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

            sb.Append($"<li>ROLE          : {player.UserRole.ToString()}</li>");
            sb.Append($"<li>VERBOSE EXITS : {player.Config.VerboseExits}</li>");
            sb.Append($"<li>BRIEF         : {player.Config.Brief}</li>");
            sb.Append($"<li>HINTS         : {player.Config.Hints}</li>");
            sb.Append($"<li>AUTOLOOT      : {player.Config.AutoLoot}</li>");
            sb.Append($"<li>TELLS         : {player.Config.Tells}</li>");
            sb.Append($"<li>WIMPY         : {player.Config.Wimpy}</li>");
            sb.Append($"<li>AUTOASSIST    : {player.Config.AutoAssist}</li>");
            sb.Append($"<li>AUTOSACRIFICE : {player.Config.AutoSacrifice}</li>");
            sb.Append($"<li>AUTOSPLIT     : {player.Config.AutoSplit}</li>");
            sb.Append($"<li>CANFOLLOW     : {player.Config.CanFollow}</li>");

            sb.Append("</ul>");

            Services.Instance.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}
