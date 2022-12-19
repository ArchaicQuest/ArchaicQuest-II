using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class HelpCmd : ICommand
    {
        public HelpCmd(ICore core)
        {
            Aliases = new[] {"help"};
            Description = "Displays the relevant help files.";
            Usages = new[] {"Type: help quaff"};
            DeniedStatus = null;
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

            if (string.IsNullOrEmpty(target))
            {
                target = "help";
            }
            
            var command = Core.Cache.GetCommand(target);
            
            if (command == null)
            {
                Core.Writer.WriteLine($"<p>No help found for {target}.", player.ConnectionId);
                return;
            }

            var sb = new StringBuilder();

            sb.Append("<div class='help-section'><table>");
            sb.Append($"<tr><td>Help Title</td><td>{target}</td></tr>");
                
            sb.Append("<tr><td>Aliases: ");
                
            var index = command.Aliases.Length;
                
            foreach (var alias in command.Aliases)
            {
                sb.Append($"{alias}");
                index--;
                if (index > 0)
                    sb.Append(", ");
            }

            sb.Append("</td></tr>");

            foreach (var usage in command.Usages)
            {
                sb.Append($"<tr><td>Last Updated</td><td>{usage}</td></tr>");
            }

            sb.Append("</table>");

            sb.Append($"<pre>{command.Description}</pre>");

            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}