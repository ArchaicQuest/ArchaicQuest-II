using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class EmoteCmd : ICommand
    {
        public EmoteCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"emote"};
            Description = "Sends a message about what your actions are";
            Usages = new[] {"Type: emote waves at wall"};
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Writer.WriteLine("Emote what?", player.ConnectionId);
                return;
            }
            
            var emoteText = string.Join(" ", input.Skip(1));
            var emoteMessage = $"{player.Name} {emoteText}";

            foreach (var players in room.Players)
            {
                Writer.WriteLine(emoteMessage, players.ConnectionId);
            }
        }
    }
}