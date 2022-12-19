using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class SmellCmd : ICommand
    {
        public SmellCmd(ICore core)
        {
            Aliases = new[] {"smell"};
            Description = "Shows how an object smells.";
            Usages = new[] {"Type: smell cupcake"};
            UserRole = UserRole.Player;
            DeniedStatus = new []
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Dead,
            };
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public ICore Core { get; }


        public void Execute(Player player, Room room, string[] input)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                Core.Writer.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }

            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                Core.Writer.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }

            var target = input.ElementAtOrDefault(1);
            
            if (string.IsNullOrEmpty(target))
            {
                Core.Writer.WriteLine("Smell what?", player.ConnectionId);
                return;
            }
            
            var nthTarget = Helpers.findNth(target);
            var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            
            if (item == null)
            {
                Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }

            var isDark = Core.RoomActions.RoomIsDark(room, player);

            Core.Writer.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Smell}",
                player.ConnectionId);

            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Core.Writer.WriteLine($"<p>{player.Name} smells {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }
            
        }
    }
}