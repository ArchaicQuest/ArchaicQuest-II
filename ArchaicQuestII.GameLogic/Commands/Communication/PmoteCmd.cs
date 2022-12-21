using System.Linq;
using System.Text.RegularExpressions;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class PmoteCmd : ICommand
    {
        public PmoteCmd(ICore core)
        {
            Aliases = new[] {"pmote"};
            Description = "Replaces mentioned player name in emote to you.";
            Usages = new[] {"Type: pmote punches steve. \n Steve will see: Bob punches You. \n Everyone else will see: Bob punches Steve."};
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
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var lastInput = input.Length;

            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                Core.Writer.WriteLine("<p>Pmote what?</p>", player.ConnectionId);
                return;
            }
            
            var emoteMessage = string.Join(" ", input.Skip(lastInput));
            var pmoteTarget = input[lastInput];
            var pattern = @"\b" + pmoteTarget + "\b";
            var replace = "you";
            var result = Regex.Replace(emoteMessage, pattern, replace);
            
            Core.Writer.WriteToOthersInRoom("<p>" + player.Name + " " + result + "</p>", room, player);
        }
    }
}