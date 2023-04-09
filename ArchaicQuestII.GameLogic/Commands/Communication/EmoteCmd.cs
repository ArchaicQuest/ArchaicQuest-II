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
        public EmoteCmd()
        {
            Aliases = new[] {"emote"};
            Description = "Sends a message about what your actions are, using a prebuilt social or a custom emote.";
            Usages = new[] {"Type: emote waves frantically and happily"};
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                CoreHandler.Instance.Writer.WriteLine("<p>Emote what?</p>", player.ConnectionId);
                return;
            }
            
            var emoteText = string.Join(" ", input.Skip(1));
            var emoteMessage = $"<p>{player.Name} {emoteText}</p>";
            
            CoreHandler.Instance.Writer.WriteToOthersInRoom(emoteMessage, room, player);
        }
    }
}