using System;
using System.Linq;
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
            Description = "";
            Usages = new[] {"Type: pmote"};
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
                Core.Writer.WriteLine("Pmote what?", player.ConnectionId);
                return;
            }
            
            var emoteMessage = string.Join(" ", input.Skip(1));

            foreach (var players in room.Players)
            {
                emoteMessage = emoteMessage.Replace(players.Name, "you", StringComparison.CurrentCultureIgnoreCase);

                Core.Writer.WriteLine(player.Name + " " + emoteMessage, players.ConnectionId);
            }
        }
    }
}