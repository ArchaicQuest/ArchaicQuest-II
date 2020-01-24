using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.API.World
{

    public class RoomController : Controller
    {

        private IDataBase _db { get; }
        public RoomController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/World/Room")]
        public void Post([FromBody] Room room)
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
                Emotes =room.Emotes,
                InstantRePop = room.InstantRePop,
                UpdateMessage = room.UpdateMessage,
                Items = room.Items,
                Mobs = room.Mobs,
                RoomObjects = room.RoomObjects,
                Type = room.Type,
                DateUpdated = DateTime.Now,
                DateCreated = DateTime.Now
            };


            _db.Save(newRoom, DataBase.Collections.Room);

        }


        [HttpGet]
        [Route("api/World/Room/{id:int}")]
        public Room Get(int id)
        {
            return _db.GetById<Room>(id, DataBase.Collections.Room);
        }

        [HttpPut]
        [Route("api/World/Room/{id:int}")]
        public void Put([FromBody] Room data)
        {
            _db.Save(data, DataBase.Collections.Room);

        }
    }
}
