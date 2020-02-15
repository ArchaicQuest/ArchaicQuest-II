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
        private IAddRoom _addRoom { get; }
        public RoomController(IDataBase db, IAddRoom addRoom)
        {
            _db = db;
            _addRoom = addRoom;
        }

        [HttpPost]
        [Route("api/World/Room")]
        public void Post([FromBody] Room room)
        {
            var newRoom = _addRoom.MapRoom(room);


            _db.Save(newRoom, DataBase.Collections.Room);

        }


        [HttpGet("{id}")]
        [Route("api/World/Room/{id:int}")]
        public Room Get(int id)
        {
            return _db.GetById<Room>(id, DataBase.Collections.Room);
        }

        [HttpPut]
        [Route("api/World/Room/{id:int}")]
        public void Put([FromBody] Room data)
        {
            var updateRoom = _addRoom.MapRoom(data);
            _db.Save(updateRoom, DataBase.Collections.Room);

        }
    }
}
