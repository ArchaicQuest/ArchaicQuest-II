using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class WhoCmd : ICommand
    {
        public WhoCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"who"};
            Description = "Displays current characters online.";
            Usages = new[] {"Type: who"};
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
            var sb = new StringBuilder();
            
            sb.Append("<ul>");
            
            foreach (var pc in Cache.GetPlayerCache())
            {
                sb.Append(
                    $"<li>[{pc.Value.Level} {pc.Value.Race} {pc.Value.ClassName}] ");
                sb.Append(
                    $"<span class='player'>{pc.Value.Name}, {pc.Value.Title}</span></li>");
            }

            sb.Append("</ul>");
            sb.Append($"<p>Players found: {Cache.GetPlayerCache().Count}</p>");

            Writer.WriteLine(sb.ToString(), player.ConnectionId);
        }
    }
}