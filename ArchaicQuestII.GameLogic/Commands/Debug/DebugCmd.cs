using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Commands.Debug
{
    public class DebugCmd : ICommand
    {
        public DebugCmd(ICore core)
        {
            Aliases = new[] {"/debug"};
            Description = "Displays debug info for the current room.";
            Usages = new[] {"Type: /debug"};
            UserRole = UserRole.Player;
            Core = core;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var jsonObject = JsonConvert.SerializeObject(room);

            Core.Writer.WriteLine(jsonObject, player.ConnectionId);
        }
    }
}
