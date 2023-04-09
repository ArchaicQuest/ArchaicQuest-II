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
        public DismountCmd()
        {
            Aliases = new[] {"dismount","dmount"};
            Description = "Use dismount to get off your mount and mount to get back on your horse, for example mount horse.";
            Usages = new[] {"Type: dismount"};
            Title = "";
            DeniedStatus = new []
            {
                CharacterStatus.Status.Sleeping,
                CharacterStatus.Status.Dead,
                CharacterStatus.Status.Busy,
                CharacterStatus.Status.Incapacitated,
                CharacterStatus.Status.Stunned
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
            if (string.IsNullOrEmpty(player.Mounted.Name))
            {
                CoreHandler.Instance.Writer.WriteLine("<p>You are not using a mount</p>");
                return;
            }

            var getMount = player.Pets.FirstOrDefault(x => x.Name.Equals(player.Mounted.Name));

            if (getMount != null)
            {
                player.Pets.Remove(getMount);
                getMount.Mounted.MountedBy = string.Empty;
                player.Mounted.Name = string.Empty;

                CoreHandler.Instance.Writer.WriteLine($"<p>You dismount {getMount.Name}.</p>", player.ConnectionId);
                CoreHandler.Instance.Writer.WriteToOthersInRoom($"<p>{player.Name} dismounts {getMount.Name}.</p>", room, player);
            }

        }
    }
}