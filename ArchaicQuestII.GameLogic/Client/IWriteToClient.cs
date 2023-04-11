using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

//copy pasta https://stackoverflow.com/a/20595549/1395510
namespace ArchaicQuestII.GameLogic.Client
{
    public interface IWriteToClient
    {
        void WriteLineMobSay(string mobName, string message, Player player);
        void WriteLine(string message, Player player, int delay = 0);
        void WriteLineAll(string message);
        void WriteToOthersInRoom(string message, Room room, Player player);
        void WriteToOthersInGame(string message, Player player);
    }
}
