using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class PoseCmd : ICommand
    {
        public PoseCmd(ICore core)
        {
            Aliases = new[] {"pose"};
            Description = "Sets your characters current pose";
            Usages = new[] {"Type: pose Leans against the wall"};
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
                var poseText = string.IsNullOrEmpty(player.LongName) ? $"<p>{ player.Name}" : $"{ player.Name} {player.LongName}";

                if (!string.IsNullOrEmpty(player.Mounted.Name))
                {
                    poseText += $", is riding {player.Mounted.Name}";
                }
                else if (string.IsNullOrEmpty(player.LongName))
                {
                    poseText += " is here";
                }

                poseText += player.Pose;

                poseText += "</p>";

                Core.Writer.WriteLine(poseText, player.ConnectionId);
                return;
            }

            player.Pose = $", {string.Join(" ", input.Skip(1))}";
            Core.Writer.WriteLine("Pose set.", player.ConnectionId);
        }
    }
}