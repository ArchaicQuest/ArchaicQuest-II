using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class WimpyCmd : ICommand
    {
        public WimpyCmd(ICore core)
        {
            Aliases = new[] {"wimpy"};
            Description = "Changes your characters auto low health flee.";
            Usages = new[] {"Type: flee 50"};
            DeniedStatus = default;
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
            var health = input.ElementAtOrDefault(1);

            var result = int.TryParse(health, out var wimpy);

            if (!result)
            {
                Core.Writer.WriteLine($"Wimpy is set to {player.Config.Wimpy}", player.ConnectionId);
                return;
            }

            if (wimpy == 0)
            {
                player.Config.Wimpy = 0;
                Core.Writer.WriteLine("Wimpy has been disabled.", player.ConnectionId);
                return;
            }
        
            if (wimpy > player.Stats.HitPoints / 3)
            {
                Core.Writer.WriteLine("Wimpy cannot be set to more than 1/3 of your max hitpoints.", player.ConnectionId);
                return;
            }

            if (wimpy < 0)
            {
                Core.Writer.WriteLine("Wimpy cannot be set to a negative.", player.ConnectionId);
                return;
            }
        
            player.Config.Wimpy = wimpy;
            Core.Writer.WriteLine($"Wimpy set to {wimpy}.", player.ConnectionId);
        }
    }
}