using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class DismountCmd : ICommand
    {
        public DismountCmd(ICore core)
        {
            Aliases = new[] {"dismount","dmount"};
            Description = "Get off your mount.";
            Usages = new[] {"Type: dismount"};
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
            if (string.IsNullOrEmpty(player.Mounted.Name))
            {
                Core.Writer.WriteLine("You are not using a mount");
                return;
            }

            var getMount = player.Pets.FirstOrDefault(x => x.Name.Equals(player.Mounted.Name));

            if (getMount != null)
            {
                player.Pets.Remove(getMount);
                getMount.Mounted.MountedBy = string.Empty;
                player.Mounted.Name = string.Empty;

                Core.Writer.WriteLine($"You dismount {getMount.Name}.", player.ConnectionId);

                foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
                {
                    Core.Writer.WriteLine($"{player.Name} dismounts {getMount.Name}.", pc.ConnectionId);
                }
            }

        }
    }
}