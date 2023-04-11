using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IRoomActions
    {
        Area.Area GetRoomArea(Room room);
        string FindValidExits(Room room, bool showVerboseExits);
        void RoomChange(Player player, Room oldRoom, Room newRoom, bool isFlee);
        Exit GetRoomExit(string exit, Room room);
    }
}
