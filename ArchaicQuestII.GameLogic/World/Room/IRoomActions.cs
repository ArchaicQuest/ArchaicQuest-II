using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IRoomActions
    {
        Area.Area GetRoomArea(Room room);
        string FindValidExits(Room room, bool showVerboseExits);
        bool RoomIsDark(Room room, Player player);
        void RoomChange(Player player, Room oldRoom, Room newRoom);
    }
}
