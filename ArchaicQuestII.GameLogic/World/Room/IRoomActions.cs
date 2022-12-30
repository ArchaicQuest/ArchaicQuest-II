using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IRoomActions
    {
        Area.Area GetRoomArea(Room room);
        bool RoomIsDark(Player player, Room room);
        string FindValidExits(Room room, bool showVerboseExits);
        void RoomChange(Player player, Room oldRoom, Room newRoom);
    }
}
