using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class HelpCmd : ICommand
    {
        public HelpCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"help"};
            Description = "Displays the description and usage.";
            Usages = new[] {"Type: help quaff"};
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

            ICommand command = null;
            
            if (string.IsNullOrEmpty(target))
            {
                Cache.GetCommand("help", out command);
            }
            else
            {
                if (!Cache.GetCommand(target, out command))
                {
                    Writer.WriteLine($"<p>No help found for {target}.", player.ConnectionId);
                    return;
                }
            }

            var sb = new StringBuilder();
            
            sb.Append($"<p>HELP FOR {target}</p>");
            sb.Append($"<p>{command.Description}</p>");
            sb.Append("<p>USAGES</p>");
            foreach (var usage in command.Usages)
            {
                sb.Append($"<p>{usage}</p>");
            }
            sb.Append("<p>ALIASES</p>");
            sb.Append("<p>");

            var index = command.Aliases.Length;
            foreach (var alias in command.Aliases)
            {
                sb.Append($"{alias}");
                index--;
                if (index > 0)
                    sb.Append(", ");
            }
            sb.Append("</p>");
            Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}