using ArchaicQuestII.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class AreaController : Controller
    {
        private IDataBase _db { get; }
        public AreaController(IDataBase db)
        {
            _db = db;
        }
        [HttpPost]
        [Route("api/World/Area")]
        public void Post([FromBody] Area area)
        {

            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid area");
                throw exception;
            }


            if (area.Id != -1)
            {

                var data = _db.GetById<Area>(area.Id, DataBase.Collections.Area);

                data.Description = area.Description;
                data.DateUpdated = DateTime.Now;
                data.Title = area.Title;

                _db.Save(data, DataBase.Collections.Area);
            }


            var newArea = new Area()
            {

                Title = area.Title,
                Description = area.Description,
                DateCreated = DateTime.Now,
                CreatedBy = "Malleus",
                DateUpdated = DateTime.Now

            };


            _db.Save(newArea, DataBase.Collections.Area);

        }

        [HttpGet]
        [Route("api/World/Area")]
        public List<Area> Get()
        {
            var areas = _db.GetList<Area>(DataBase.Collections.Area);

            foreach (var area in areas)
            {
                var rooms = _db.GetCollection<Room>(DataBase.Collections.Room).Find(x => x.AreaId == area.Id);
                area.Rooms = rooms.ToList();
            }

            return areas;
        }

        [HttpGet]
        [Route("api/World/Area/{id:int}")]
        public Area Get(int id)
        {
            var area = _db.GetById<Area>(id, DataBase.Collections.Area);
            var rooms = _db.GetCollection<Room>(DataBase.Collections.Room).Find(x => x.AreaId == id);

            area.Rooms = rooms.ToList();

            return area;
        }



        private bool CheckIfValidFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".json");
        }
        public async Task<string> ReadAsStringAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    result.AppendLine(await reader.ReadLineAsync());
                }
            }
            return result.ToString();
        }

        [HttpPost]
        [Route("api/World/Area/UploadArea")]
        public async Task<IActionResult> UploadArea([FromForm(Name = "file")] IFormFile jsonString)
        {

            if (CheckIfValidFile(jsonString))
            {
                using (var reader = new StreamReader(jsonString.OpenReadStream()))
                {
                    var x = await reader.ReadToEndAsync();

                    var exitingArea = JsonConvert.DeserializeObject<Area>(x);
                    var editExistingArea = JsonConvert.DeserializeObject<Area>(x);

                    // server may already issued the ids
                    // so we will remove them and have 
                    // the DB issue new ones

                    editExistingArea.Id = _db.GetList<Area>(DataBase.Collections.Area).Count + 1;
                    editExistingArea.DateCreated = DateTime.Now;
                    editExistingArea.Rooms = new List<Room>();

                    // Add the new area first
                    _db.Save(editExistingArea, DataBase.Collections.Area);

                    //create rooms
                    foreach (var room in exitingArea.Rooms)
                    {
                        room.Id = _db.GetList<Room>(DataBase.Collections.Room).Count + 1;
                        room.AreaId = editExistingArea.Id;
                        if (room.Exits.Down != null)
                        {
                            room.Exits.Down.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.Up != null)
                        {
                            room.Exits.Up.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.North != null)
                        {
                            room.Exits.North.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.NorthEast != null)
                        {
                            room.Exits.NorthEast.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.NorthWest != null)
                        {
                            room.Exits.NorthWest.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.East != null)
                        {
                            room.Exits.East.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.SouthEast != null)
                        {
                            room.Exits.SouthEast.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.South != null)
                        {
                            room.Exits.South.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.SouthWest != null)
                        {
                            room.Exits.SouthWest.AreaId = editExistingArea.Id;
                        }
                        if (room.Exits.West != null)
                        {
                            room.Exits.West.AreaId = editExistingArea.Id;
                        }

                        //do exits
                        var newRooms = _db.GetCollection<Room>(DataBase.Collections.Room).FindAll().Where(x => x.AreaId == editExistingArea.Id);
                        foreach (var rm in newRooms)
                        {
                            if (room.Exits.Down != null)
                            {
                                rm.Exits.Down.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.Down.Coords.X && x.Coords.Y == rm.Exits.Down.Coords.Y &&
                                    x.Coords.Z == rm.Exits.Down.Coords.Z).Id;
                            }
                            if (room.Exits.Up != null)
                            {
                                rm.Exits.Up.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.Up.Coords.X && x.Coords.Y == rm.Exits.Up.Coords.Y &&
                                    x.Coords.Z == rm.Exits.Up.Coords.Z).Id;
                            }
                            if (room.Exits.North != null)
                            {
                                rm.Exits.North.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.North.Coords.X && x.Coords.Y == rm.Exits.North.Coords.Y &&
                                    x.Coords.Z == rm.Exits.North.Coords.Z).Id;
                            }
                            if (room.Exits.NorthEast != null)
                            {
                                rm.Exits.NorthEast.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.NorthEast.Coords.X && x.Coords.Y == rm.Exits.NorthEast.Coords.Y &&
                                    x.Coords.Z == rm.Exits.NorthEast.Coords.Z).Id;
                            }
                            if (room.Exits.NorthWest != null)
                            {
                                rm.Exits.NorthWest.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.NorthWest.Coords.X && x.Coords.Y == rm.Exits.NorthWest.Coords.Y &&
                                    x.Coords.Z == rm.Exits.NorthWest.Coords.Z).Id;
                            }
                            if (room.Exits.East != null)
                            {
                                rm.Exits.East.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.East.Coords.X && x.Coords.Y == rm.Exits.East.Coords.Y &&
                                    x.Coords.Z == rm.Exits.East.Coords.Z).Id;
                            }
                            if (room.Exits.SouthEast != null)
                            {
                                rm.Exits.SouthEast.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.SouthEast.Coords.X && x.Coords.Y == rm.Exits.SouthEast.Coords.Y &&
                                    x.Coords.Z == rm.Exits.SouthEast.Coords.Z).Id;
                            }
                            if (room.Exits.South != null)
                            {
                                rm.Exits.South.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.South.Coords.X && x.Coords.Y == rm.Exits.South.Coords.Y &&
                                    x.Coords.Z == rm.Exits.South.Coords.Z).Id;
                            }
                            if (room.Exits.SouthWest != null)
                            {
                                rm.Exits.SouthWest.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.SouthWest.Coords.X && x.Coords.Y == rm.Exits.SouthWest.Coords.Y &&
                                    x.Coords.Z == rm.Exits.SouthWest.Coords.Z).Id;
                            }
                            if (room.Exits.West != null)
                            {
                                rm.Exits.West.RoomId = newRooms.FirstOrDefault(x =>
                                    x.Coords.X == rm.Exits.West.Coords.X && x.Coords.Y == rm.Exits.West.Coords.Y &&
                                    x.Coords.Z == rm.Exits.West.Coords.Z).Id;
                            }
                        }

                        _db.Save(room, DataBase.Collections.Room);
                    }


                }

            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }

            return Ok();

        }


        [HttpPut]
        [Route("api/World/Area/{id:int}")]
        public void Put([FromBody] Area data)
        {
            var area = _db.GetById<Area>(data.Id, DataBase.Collections.Area);

            area.Description = area.Description;
            area.DateUpdated = DateTime.Now;
            area.Title = area.Title;

            _db.Save(data, DataBase.Collections.Area);

        }

    }
}
