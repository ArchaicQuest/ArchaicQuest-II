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
                Terrain = room.Terrain,
                DateUpdated = DateTime.Now,
                DateCreated = DateTime.Now,
                
            };

            MapRoomId(newRoom);

                if(room.Id != -1)
            {
                newRoom.Id = room.Id;
            }

            return newRoom;

        }
        public void MapRoomId(Room room)
        {
            var northRoom = room.Exits.North?.Coords;
            if (northRoom != null)
            {
                room.Exits.North.RoomId = GetRoomFromCoords(northRoom, room.AreaId) != null ? GetRoomFromCoords(northRoom, room.AreaId).Id : -1;
            }

            var eastRoom = room.Exits.East?.Coords;
            if (eastRoom != null)
            {
                room.Exits.East.RoomId = GetRoomFromCoords(eastRoom, room.AreaId) != null ? GetRoomFromCoords(eastRoom, room.AreaId).Id : -1;
            }

            var southRoom = room.Exits.South?.Coords;
            if (southRoom != null)
            {
                room.Exits.South.RoomId = GetRoomFromCoords(southRoom, room.AreaId) != null ? GetRoomFromCoords(southRoom, room.AreaId).Id : -1;
            }

            var westRoom = room.Exits.West?.Coords;
            if (westRoom != null)
            {
                room.Exits.West.RoomId = GetRoomFromCoords(westRoom, room.AreaId) != null ? GetRoomFromCoords(westRoom, room.AreaId).Id : -1;
            }

            var NWRoom = room.Exits.NorthWest?.Coords;
            if (NWRoom != null)
            {
                room.Exits.NorthWest.RoomId = GetRoomFromCoords(NWRoom, room.AreaId) != null ? GetRoomFromCoords(NWRoom, room.AreaId).Id : -1;
            }

            var NERoom = room.Exits.NorthEast?.Coords;
            if (NERoom != null)
            {
                room.Exits.NorthEast.RoomId = GetRoomFromCoords(NERoom, room.AreaId) != null ? GetRoomFromCoords(NERoom, room.AreaId).Id : -1;
            }

            var SERoom = room.Exits.SouthEast?.Coords;
            if (SERoom != null)
            {
                room.Exits.SouthEast.RoomId = GetRoomFromCoords(SERoom, room.AreaId) != null ? GetRoomFromCoords(SERoom, room.AreaId).Id : -1;
            }

            var SWRoom = room.Exits.SouthWest?.Coords;
            if (SWRoom != null)
            {
                room.Exits.SouthWest.RoomId = GetRoomFromCoords(SWRoom, room.AreaId) != null ? GetRoomFromCoords(SWRoom, room.AreaId).Id : -1;
            }

            var DRoom = room.Exits.Down?.Coords;
            if (DRoom != null)
            {
                room.Exits.Down.RoomId = GetRoomFromCoords(DRoom, room.AreaId) != null ? GetRoomFromCoords(DRoom, room.AreaId).Id : -1;
            }

            var URoom = room.Exits.Up?.Coords;
            if (URoom != null)
            {
                room.Exits.Up.RoomId = GetRoomFromCoords(URoom, room.AreaId) != null ? GetRoomFromCoords(URoom, room.AreaId).Id : -1;
            }
        }

        public Room GetRoomFromCoords(Coordinates coords, int areaId)
        {
            return _db.GetCollection<Room>(DataBase.Collections.Room).FindOne(x => x.AreaId.Equals(areaId) && x.Coords.X.Equals(coords.X) && x.Coords.Y.Equals(coords.Y) && x.Coords.Z.Equals(coords.Z));
        }
    }
}
