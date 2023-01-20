using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.World;

public interface IWorldHandler
{
    void Init();
    Time Time { get; }
    void ClearCache();
    bool AddRoom(string id, Room.Room room);
    bool AddOriginalRoom(string id, Room.Room room);
    Room.Room GetRoom(string id);
    Room.Room GetOriginalRoom(string id);
    List<Room.Room> GetAllRooms();
    List<Room.Room> GetOriginalRooms();
    List<Room.Room> GetAllRoomsInArea(int id);
    List<Room.Room> GetAllRoomsToRepop();
    bool UpdateRoom(string id, Room.Room room, Player player);
    bool RoomIsDark(Player player, Room.Room room);
    void AddMap(string areaId, string map);
    string GetMap(string areaId);
    string SimulateWeatherTransitions();
    void DisplayTimeOfDayMessage();
    Room.Room MapRoom(Room.Room room);
    void MapRoomId(Room.Room room);
    Room.Room GetRoomFromCoords(Coordinates coords, int areaId);
    Area.Area GetRoomArea(Room.Room room);
    string FindValidExits(Room.Room room, bool verbose);
    void RoomChange(Player player, Room.Room oldRoom, Room.Room newRoom);
    List<Area.Area> GetAllAreas();
}