using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class DismountCmd : ICommand
    {
        public DismountCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"dismount","dmount"};
            Description = "Get off your mount.";
            Usages = new[] {"Type: dismount"};
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
            if (string.IsNullOrEmpty(player.Mounted.Name))
            {
                Writer.WriteLine("You are not using a mount");
                return;
            }

            var getMount = player.Pets.FirstOrDefault(x => x.Name.Equals(player.Mounted.Name));

            if (getMount != null)
            {
                player.Pets.Remove(getMount);
                getMount.Mounted.MountedBy = string.Empty;
                player.Mounted.Name = string.Empty;

                Writer.WriteLine($"You dismount {getMount.Name}.", player.ConnectionId);

                foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
                {
                    Writer.WriteLine($"{player.Name} dismounts {getMount.Name}.", pc.ConnectionId);
                }
            }

        }
    }
}