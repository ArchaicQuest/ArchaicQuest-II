using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Commands.Debug
{
    public class DebugCmd : ICommand
    {
        public DebugCmd(IWriteToClient writeToClient, Cache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
        {
            Aliases = new[] {"/debug"};
            Description = "Displays debug info for the current room.";
            Usages = new[] {"Type: /debug"};
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
            var jsonObject = JsonConvert.SerializeObject(room);

            Writer.WriteLine(jsonObject, player.ConnectionId);
        }
    }
}
