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
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// For more inforoomation on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{

    [Authorize]
    public class AreaController : ControllerBase
    {
        private IDataBase _db { get; }
        public AreaController(IDataBase db)
        {
            _db = db;
        }
        [HttpPost]
        [Route("api/World/Area")]
        public IActionResult Post([FromBody] Area area)
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
            var user = (HttpContext.Items["User"] as AdminUser);

            var newArea = new Area()
            {

                Title = area.Title,
                Description = area.Description,
                DateCreated = DateTime.Now,
                CreatedBy = user?.Username ?? "Malleus",
                DateUpdated = DateTime.Now

            };


            _db.Save(newArea, DataBase.Collections.Area);


            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({newArea.Id}) {newArea.Title}",
                Type = DataBase.Collections.Room,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
            return Ok(JsonConvert.SerializeObject(new { toast = $"Area saved successfully." }));

        }


        //[HttpDelete]
        //[Route("api/World/Area/{id:int}")]
        //public IActionResult Delete(int id)
        //{
        //    var deleted = _db.Delete<Area>(id, DataBase.Collections.Area);
        //   // var deletedRooms = _db.Delete<Room>(id, DataBase.Collections.Area);

        //    if (deleted == false)
        //    {
        //        return Ok(JsonConvert.SerializeObject(new { toast = $"ERROR: Room ${id} failed to delete." }));
        //    }

        //    return Ok(JsonConvert.SerializeObject(new { toast = $"Room deleted successfully." }));

        //}

        [HttpGet]
        [Route("api/World/Area")]
        public List<Area> Get()
        {
            var areas = _db.GetList<Area>(DataBase.Collections.Area);

            foreach (var area in areas)
            {
                var rooms = _db.GetCollection<Room>(DataBase.Collections.Room).Find(x => x.AreaId == area.Id && x.Deleted == false);
                area.Rooms = rooms.ToList();
            }

            return areas;
        }

        [HttpGet]
        [Route("api/World/Area/{id:int}")]
        public Area Get(int id)
        {
            var area = _db.GetById<Area>(id, DataBase.Collections.Area);
            var rooms = _db.GetCollection<Room>(DataBase.Collections.Room).Find(x => x.AreaId == id && x.Deleted == false);

            area.Rooms = rooms.ToList();

            return area;
        }

        [HttpGet]
        [Route("api/World/Area/List")]
        public List<string> GetListOfAreas()
        {
            var areaCollection = _db.GetCollection<Area>(DataBase.Collections.Area).FindAll();

            return areaCollection.Select(area => area.Title).ToList();
        }



        private bool CheckIfValidFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".json");
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

                    editExistingArea.Id = _db.GetList<Area>(DataBase.Collections.Area).Count + 2;
                    editExistingArea.DateCreated = DateTime.Now;
                    editExistingArea.Rooms = new List<Room>();

                    // Add the new area first
                    _db.Save(editExistingArea, DataBase.Collections.Area);



                    //create rooms
                    foreach (var room in exitingArea.Rooms)
                    {
                        room.Id = exitingArea.Rooms.Max(x => x.Id) + 1;
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
                        _db.Save(room, DataBase.Collections.Room);

                    }

                    //do exits
                    var newRooms = _db.GetCollection<Room>(DataBase.Collections.Room).FindAll().Where(x => x.AreaId == editExistingArea.Id);
                    foreach (var room in newRooms)
                    {
                        if (room.Exits.Down != null)
                        {
                            room.Exits.Down.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.Down.Coords.X && x.Coords.Y == room.Exits.Down.Coords.Y &&
                                x.Coords.Z == room.Exits.Down.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.Up != null)
                        {
                            room.Exits.Up.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.Up.Coords.X && x.Coords.Y == room.Exits.Up.Coords.Y &&
                                x.Coords.Z == room.Exits.Up.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.North != null)
                        {
                            room.Exits.North.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.North.Coords.X && x.Coords.Y == room.Exits.North.Coords.Y &&
                                x.Coords.Z == room.Exits.North.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.NorthEast != null)
                        {
                            room.Exits.NorthEast.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.NorthEast.Coords.X && x.Coords.Y == room.Exits.NorthEast.Coords.Y &&
                                x.Coords.Z == room.Exits.NorthEast.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.NorthWest != null)
                        {
                            room.Exits.NorthWest.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.NorthWest.Coords.X && x.Coords.Y == room.Exits.NorthWest.Coords.Y &&
                                x.Coords.Z == room.Exits.NorthWest.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.East != null)
                        {
                            room.Exits.East.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.East.Coords.X && x.Coords.Y == room.Exits.East.Coords.Y &&
                                x.Coords.Z == room.Exits.East.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.SouthEast != null)
                        {
                            room.Exits.SouthEast.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.SouthEast.Coords.X && x.Coords.Y == room.Exits.SouthEast.Coords.Y &&
                                x.Coords.Z == room.Exits.SouthEast.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.South != null)
                        {
                            room.Exits.South.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.South.Coords.X && x.Coords.Y == room.Exits.South.Coords.Y &&
                                x.Coords.Z == room.Exits.South.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.SouthWest != null)
                        {
                            room.Exits.SouthWest.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.SouthWest.Coords.X &&
                                x.Coords.Y == room.Exits.SouthWest.Coords.Y &&
                                x.Coords.Z == room.Exits.SouthWest.Coords.Z)?.Id ?? -1;
                        }
                        if (room.Exits.West != null)
                        {
                            room.Exits.West.RoomId = newRooms.FirstOrDefault(x =>
                                x.Coords.X == room.Exits.West.Coords.X && x.Coords.Y == room.Exits.West.Coords.Y &&
                                x.Coords.Z == room.Exits.West.Coords.Z)?.Id ?? -1;
                        }
                        room.RoomObjects.RemoveAll(x => x.Name == null);
                        _db.Save(room, DataBase.Collections.Room);
                    }

                    //save mobs
                    foreach (var room in newRooms)
                    {
                        // if a room has 4 goblins don't add 4 goblins to the mob list
                        foreach (var mob in room.Mobs.GroupBy(x => x.Name).Select(x => x.First()).ToList())
                        {
                            _db.Save(mob, DataBase.Collections.Mobs);
                        }
                    }

                    //save items
                    foreach (var room in newRooms)
                    {
                        foreach (var item in room.Items.GroupBy(x => x.Name).Select(x => x.First()).ToList())
                        {
                            _db.Save(item, DataBase.Collections.Items);
                        }
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

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({area.Id}) {area.Title}",
                Type = DataBase.Collections.Area,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);

        }

        [HttpDelete]
        [Route("api/World/Area/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var area = _db.GetCollection<Area>(DataBase.Collections.Area).FindById(id);
            area.Deleted = true;
            var saved = _db.Save(area, DataBase.Collections.Mobs);

            if (saved)
            {
                var user = (HttpContext.Items["User"] as AdminUser);
                var log = new AdminLog()
                {
                    Detail = $"Deleted ({area.Id}) {area.Title}",
                    Type = DataBase.Collections.Area,
                    UserName = user.Username
                };
                _db.Save(log, DataBase.Collections.Log);
                return Ok(JsonConvert.SerializeObject(new { toast = $"{area.Title} deleted successfully." }));
            }
            return Ok(JsonConvert.SerializeObject(new { toast = $"{area.Title} deletion failed." }));

        }

    }
}
