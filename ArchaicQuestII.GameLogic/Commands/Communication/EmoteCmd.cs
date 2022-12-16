using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class EmoteCmd : ICommand
    {
        public EmoteCmd(ICore core)
        {
            Aliases = new[] {"emote"};
            Description = "Sends a message about what your actions are";
            Usages = new[] {"Type: emote waves at wall"};
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Core.Writer.WriteLine("Emote what?", player.ConnectionId);
                return;
            }
            
            var emoteText = string.Join(" ", input.Skip(1));
            var emoteMessage = $"{player.Name} {emoteText}";

            foreach (var players in room.Players)
            {
                Core.Writer.WriteLine(emoteMessage, players.ConnectionId);
            }
        }
    }
}