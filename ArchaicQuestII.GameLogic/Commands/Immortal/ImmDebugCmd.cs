using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;

namespace ArchaicQuestII.GameLogic.Commands.Immortal
{
    public class ImmDebugCmd : ICommand
    {
        public ImmDebugCmd(ICore core)
        {
            Aliases = new[] {"/debug"};
            Description = "Displays debug info for the current room.";
            Usages = new[] {"Type: /debug"};
            DeniedStatus = null;
            UserRole = UserRole.Staff;
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
            var jsonObject = JsonConvert.SerializeObject(room);

            Core.Writer.WriteLine(jsonObject, player.ConnectionId);
        }
    }
}
