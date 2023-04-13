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
        public ImmDebugCmd()
        {
            Aliases = new[] { "/debug" };
            Description = "Displays debug info for the current room.";
            Usages = new[] { "Type: /debug" };
            Title = "";
            DeniedStatus = null;
            UserRole = UserRole.Staff;
        }

        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public string Title { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public UserRole UserRole { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            var jsonObject = JsonConvert.SerializeObject(room);

            Services.Instance.Writer.WriteLine(jsonObject, player);
        }
    }
}
