using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character
{
    public class SaveCmd : ICommand
    {
        public SaveCmd(ICore core)
        {
            Aliases = new[] {"save"};
            Description = "Save your character manually";
            Usages = new[] {"Type: save"};
            UserRole = UserRole.Player;
            DeniedStatus = default;
            Core = core;
        }
        
        public string[] Aliases { get; }
        public string Description { get; }
        public string[] Usages { get; }
        public UserRole UserRole { get; }
        public CharacterStatus.Status[] DeniedStatus { get; }
        public ICore Core { get; }

        public void Execute(Player player, Room room, string[] input)
        {
            Core.PlayerDataBase.Save(player, PlayerDataBase.Collections.Players);
            Core.Writer.WriteLine("Character saved.", player.ConnectionId);
        }
    }
}