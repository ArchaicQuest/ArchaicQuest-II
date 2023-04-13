using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands
{
    public interface ICommandHandler
    {
        public void HandleCommand(Player player, Room room, string input);
    }
}
