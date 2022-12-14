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
                Writer.WriteLine("Pose how?", player.ConnectionId);
                return;
            }
            
            var poseText = string.Join(" ", input.Skip(1));
            
            if (string.IsNullOrEmpty(poseText))
            {
                player.Pose = poseText;
            }
            
            player.Pose = $", {poseText}";
            Writer.WriteLine("Pose set.", player.ConnectionId);
        }
    }
}