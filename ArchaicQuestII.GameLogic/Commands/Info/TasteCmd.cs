using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Info
{
    public class TasteCmd : ICommand
    {
        public TasteCmd(ICore core)
        {
            Aliases = new[] {"taste"};
            Description = "Shows how an object tasts.";
            Usages = new[] {"Type: smell cupcake"};
            UserRole = UserRole.Player;
            DeniedStatus = default;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }


        public void Execute(Player player, Room room, string[] input)
        {
            if (player.Status == CharacterStatus.Status.Sleeping)
            {
                Core.Writer.WriteLine("You can't do that while asleep.", player.ConnectionId);
                return;
            }
            var target = input.ElementAtOrDefault(1);

            if (string.IsNullOrEmpty(target))
            {
                Core.Writer.WriteLine("Taste what?", player.ConnectionId);
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

            Core.Writer.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Taste}",
                player.ConnectionId);
            
            foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
            {
                Core.Writer.WriteLine($"<p>{player.Name} tastes {item.Name.ToLower()}.</p>", pc.ConnectionId);
            }
        }
    }
}