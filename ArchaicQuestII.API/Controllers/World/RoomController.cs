using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.API.World
{

    public class RoomController : Controller
    {

        private IDataBase _db { get; }
        private IAddRoom _addRoom { get; }
        private ICache _cache { get; }
        public RoomController(IDataBase db, IAddRoom addRoom, ICache cache)
        {
            _db = db;
            _addRoom = addRoom;
            _cache = cache;
        }

        [HttpPost]
        [Route("api/World/Room")]
        public void Post([FromBody] Room room)
        {
            var newRoom = _addRoom.MapRoom(room);


            _db.Save(newRoom, DataBase.Collections.Room);

        }

        [HttpDelete]
        [Route("api/World/Room/{id:int}")]
        public IActionResult Delete(int id)
        {
          var deleted = _db.Delete<Room>(id, DataBase.Collections.Room);

            if (deleted == false)
            {
                return Ok(JsonConvert.SerializeObject(new { toast = $"ERROR: Room ${id} failed to delete." }));
            }

            return Ok(JsonConvert.SerializeObject(new { toast = $"Room deleted successfully." }));

        }


        [HttpGet("{id}")]
        [Route("api/World/Room/{id:int}")]
        public Room Get(int id)
        {
            return _db.GetById<Room>(id, DataBase.Collections.Room);
        }

        ///
        
        [HttpGet]
        [Route("api/World/Room/returnRoomTypes")]
        public JsonResult ReturnRoomTypes()
        {

            var roomTypes = new List<object>();

            foreach (var item in Enum.GetValues(typeof(Room.RoomType)))
            {

                roomTypes.Add(new
                {
                    id = (int)item,
                    name = item.ToString()
                });
            }
            return Json(roomTypes);

        }
        [HttpPut]
        [Route("api/World/Room/{id:int}")]
        public void Put([FromBody] Room data)
        {
            var updateRoom = _addRoom.MapRoom(data);
            _db.Save(updateRoom, DataBase.Collections.Room);

        }

        [HttpGet("{x}/{y}/{z}/{areaId}")]
        [Route("api/World/Room/{x:int}/{y:int}/{z:int}/{areaId:int}")]
        public bool validExit(int x, int y, int z, int areaId)
        {
            return _addRoom.GetRoomFromCoords(new Coordinates { X = x, Y = y, Z = z }, areaId) != null;
        }

        [HttpPost]
        [Route("api/World/Room/updateCache")]
        public IActionResult UpdateRoomCache()
        {

            Stopwatch s = Stopwatch.StartNew();

            _cache.ClearRoomCache();

            var rooms = _db.GetList<Room>(DataBase.Collections.Room);

            foreach (var room in rooms)
            {
                _cache.AddRoom(room.Id, room);
            }

            s.Stop();

            return Ok(JsonConvert.SerializeObject(new { toast = $"Room cache updated successfully. Elapsed Time: {s.ElapsedMilliseconds} ms" }));
        }

      
}
}
