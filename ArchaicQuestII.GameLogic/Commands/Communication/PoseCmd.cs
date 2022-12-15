using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication
{
    public class PoseCmd : ICommand
    {
        public PoseCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"pose"};
            Description = "Sets your characters current pose";
            Usages = new[] {"Type: pose Leans against the wall"};
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
            if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
            {
                var poseText = string.IsNullOrEmpty(player.LongName) ? $"{ player.Name}" : $"{ player.Name} {player.LongName}";

                if (!string.IsNullOrEmpty(player.Mounted.Name))
                {
                    poseText += $", is riding {player.Mounted.Name}";
                }
                else if (string.IsNullOrEmpty(player.LongName))
                {
                    poseText += " is here";
                }

                poseText += player.Pose;

                Writer.WriteLine(poseText, player.ConnectionId);
                return;
            }

            player.Pose = $", {string.Join(" ", input.Skip(1))}";
            Writer.WriteLine("Pose set.", player.ConnectionId);
        }
    }
}