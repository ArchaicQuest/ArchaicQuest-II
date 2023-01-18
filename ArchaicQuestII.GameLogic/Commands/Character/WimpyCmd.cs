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
        public WimpyCmd(ICoreHandler coreHandler)
        {
            Aliases = new[] {"wimpy"};
            Description = "Wimpy sets your wimpy value.  When your character takes damage that reduces " +
                          "your hit points below your wimpy value, you will automatically attempt to flee.";
            Usages = new[] {"Type: wimpy 50"};
            Title = "";
            DeniedStatus = default;
            UserRole = UserRole.Player;

            Handler = coreHandler;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }
        
        public ICoreHandler Handler { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var health = input.ElementAtOrDefault(1);

            var result = int.TryParse(health, out var wimpy);

            if (!result)
            {
                Handler.Client.WriteLine($"<p>Wimpy is set to {player.Config.Wimpy}.</p>", player.ConnectionId);
                return;
            }

            if (wimpy == 0)
            {
                player.Config.Wimpy = 0;
                Handler.Client.WriteLine("<p>Wimpy has been disabled.</p>", player.ConnectionId);
                return;
            }
        
            if (wimpy > player.Stats.HitPoints / 3)
            {
                Handler.Client.WriteLine("<p>Wimpy cannot be set to more than 1/3 of your max hitpoints.</p>", player.ConnectionId);
                return;
            }

            if (wimpy < 0)
            {
                Handler.Client.WriteLine("<p>Wimpy cannot be set to a negative.</p>", player.ConnectionId);
                return;
            }
        
            player.Config.Wimpy = wimpy;
            Handler.Client.WriteLine($"<p>Wimpy set to {wimpy}.</p>", player.ConnectionId);
        }
    }
}