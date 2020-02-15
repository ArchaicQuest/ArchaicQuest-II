using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public interface IAddRoom
    {

        Room MapRoom(Room room);
        void MapRoomId(Room room);
        Room GetRoomFromCoords(Coordinates coords);
    }
}
