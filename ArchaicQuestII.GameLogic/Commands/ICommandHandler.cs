using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommandHandler
    {
        public ICore Core { get; }
        public void HandleCommand(Player player, Room room, string input);
    }
}
