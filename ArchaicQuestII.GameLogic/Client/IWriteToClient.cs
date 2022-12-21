using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

//copy pasta https://stackoverflow.com/a/20595549/1395510
namespace ArchaicQuestII.GameLogic.Client
{
    public interface IWriteToClient
    {
        void WriteLineMobSay(string mobName, string message, string id);
        void WriteLine(string message, string id);
        void WriteLine(string message, string id, int delay);
        void WriteLine(string message);
        void WriteLineRoom(string message, string id, int delay);
        void WriteToOthersInRoom(string message, Room room, Player player);
    }
}
