using ArchaicQuestII.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class AddRoom: IAddRoom
    {

        private IDataBase _db { get; }
        public AddRoom(IDataBase db)
        {
            _db = db;

        }

        public Room MapRoom(Room room)
        {
            var newRoom = new Room()
            {
                Title = room.Title,
                Description = room.Description,
                AreaId = room.AreaId,
                Coords = new Coordinates()
                {
                    X = room.Coords.X,
                    Y = room.Coords.Y,
                    Z = room.Coords.Z
                },
                Exits = room.Exits,
                Emotes = room.Emotes,
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Items = room.Items,
                Mobs = room.Mobs,
                RoomObjects = room.RoomObjects,
                Type = room.Type,
                DateUpdated = DateTime.Now,
                DateCreated = DateTime.Now
            };

            MapRoomId(newRoom);

            return newRoom;

        }
        public void MapRoomId(Room room)
        {
            var northRoom = room.Exits.North?.Coords;
            if (northRoom != null)
            {
                room.Exits.North.RoomId = GetRoomFromCoords(northRoom) != null ? GetRoomFromCoords(northRoom).Id : -1;
            }

            var eastRoom = room.Exits.East?.Coords;
            if (eastRoom != null)
            {
                room.Exits.East.RoomId = GetRoomFromCoords(eastRoom) != null ? GetRoomFromCoords(eastRoom).Id : -1;

            }
        }

        public Room GetRoomFromCoords(Coordinates coords)
        {
            return _db.GetCollection<Room>(DataBase.Collections.Room).FindOne(x => x.Coords.X.Equals(coords.X) && x.Coords.Y.Equals(coords.Y) && x.Coords.Z.Equals(coords.Z));
        }
    }
}
