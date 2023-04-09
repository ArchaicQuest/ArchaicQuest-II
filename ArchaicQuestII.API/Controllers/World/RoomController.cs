using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Model;
using Newtonsoft.Json;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Character;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.API.World
{
    [Authorize]
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
        public IActionResult Post([FromBody] Room room)
        {
            var newRoom = _addRoom.MapRoom(room);

            _db.Save(newRoom, DataBase.Collections.Room);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            return Ok(JsonConvert.SerializeObject(new { toast = $"Room saved successfully." }));
        }

        //[HttpDelete]
        //[Route("api/World/Room/{id:int}")]
        //public IActionResult Delete(int id)
        //{
        //  var deleted = _db.Delete<Room>(id, DataBase.Collections.Room);

        //    if (deleted == false)
        //    {
        //        return Ok(JsonConvert.SerializeObject(new { toast = $"ERROR: Room ${id} failed to delete." }));
        //    }

        //    return Ok(JsonConvert.SerializeObject(new { toast = $"Room deleted successfully." }));

        //}


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
                roomTypes.Add(new { id = (int)item, name = item.ToString() });
            }
            return Json(roomTypes);
        }

        [HttpPut]
        [Route("api/World/Room/{id:int}")]
        public void Put([FromBody] Room data)
        {
            var updateRoom = _addRoom.MapRoom(data);
            _db.Save(updateRoom, DataBase.Collections.Room);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail =
                    $"({data.AreaId}, {data.Id}, x: {data.Coords.X} y: {data.Coords.Y}, z: {data.Coords.Z}) {data.Title}",
                Type = DataBase.Collections.Room,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
        }

        [HttpGet("{x}/{y}/{z}/{areaId}")]
        [Route("api/World/Room/{x:int}/{y:int}/{z:int}/{areaId:int}")]
        public bool validExit(int x, int y, int z, int areaId)
        {
            return _addRoom.GetRoomFromCoords(
                    new Coordinates
                    {
                        X = x,
                        Y = y,
                        Z = z
                    },
                    areaId
                ) != null;
        }

        public void MapMobRoomId(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                mob.UniqueId = Guid.NewGuid();
                mob.RoomId = $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}";
            }
        }

        public void AddSkillsToMobs(Room room)
        {
            foreach (var mob in room.Mobs)
            {
                mob.AddSkills(Enum.Parse<ClassName>(mob.ClassName));

                foreach (var skill in mob.Skills)
                {
                    skill.Proficiency = 100;
                }

                //set mob armor
                mob.ArmorRating = new ArmourRating()
                {
                    Armour = mob.Level > 5 ? mob.Level * 3 : 1,
                    Magic = mob.Level * 3 / 4,
                };

                //give mob unique IDs
                mob.UniqueId = Guid.NewGuid();
            }
        }

        [HttpPost]
        [Route("api/World/Room/updateCache")]
        public IActionResult UpdateRoomCache()
        {
            Stopwatch s = Stopwatch.StartNew();

            var roomsWithPlayers = _cache.GetAllRooms().Where(x => x.Players.Any());

            _cache.ClearRoomCache();

            try
            {
                var rooms = _db.GetList<Room>(DataBase.Collections.Room);

                foreach (var room in rooms.Where(x => x.Deleted == false))
                {
                    AddSkillsToMobs(room);
                    MapMobRoomId(room);

                    var roomHasPlayers = roomsWithPlayers.FirstOrDefault(x => x.Id.Equals(room.Id));

                    if (roomHasPlayers != null)
                    {
                        foreach (var player in roomHasPlayers.Players)
                        {
                            room.Players.Add(player);
                        }
                    }

                    _cache.AddRoom(
                        $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                        room
                    );
                    _cache.AddOriginalRoom(
                        $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                        JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(room))
                    );
                }

                var areas = _db.GetList<Area>(DataBase.Collections.Area);

                foreach (var area in areas)
                {
                    var roomList = rooms.FindAll(x => x.AreaId == area.Id);
                    var areaByZIndex = roomList.FindAll(x => x.Coords.Z != 0).Distinct();
                    foreach (var zarea in areaByZIndex)
                    {
                        var roomsByZ = new List<Room>();
                        foreach (var room in roomList.FindAll(x => x.Coords.Z == zarea.Coords.Z))
                        {
                            roomsByZ.Add(room);
                        }

                        _cache.AddMap($"{area.Id}{zarea.Coords.Z}", Map.DrawMap(roomsByZ));
                    }

                    var rooms0index = roomList.FindAll(x => x.Coords.Z == 0);
                    _cache.AddMap($"{area.Id}0", Map.DrawMap(rooms0index));
                }

                var helps = _db.GetList<Help>(DataBase.Collections.Help);

                foreach (var help in helps)
                {
                    _cache.AddHelp(help.Id, help);
                }

                var quests = _db.GetList<Quest>(DataBase.Collections.Quests);

                foreach (var quest in quests)
                {
                    _cache.AddQuest(quest.Id, quest);
                }

                var skills = _db.GetList<Skill>(DataBase.Collections.Skill);

                foreach (var skill in skills)
                {
                    _cache.AddSkill(skill.Id, skill);
                }

                var craftingRecipes = _db.GetList<CraftingRecipes>(
                    DataBase.Collections.CraftingRecipes
                );
                foreach (var craftingRecipe in craftingRecipes)
                {
                    _cache.AddCraftingRecipes(craftingRecipe.Id, craftingRecipe);
                }
            }
            catch (Exception) { }

            s.Stop();

            return Ok(
                JsonConvert.SerializeObject(
                    new
                    {
                        toast = $"Room and Map cache updated successfully. Elapsed Time: {s.ElapsedMilliseconds} ms"
                    }
                )
            );
        }

        [HttpDelete]
        [Route("api/World/Room/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var room = _db.GetCollection<Room>(DataBase.Collections.Room).FindById(id);
            _db.Delete<Room>(id, DataBase.Collections.Room);

            return Ok(
                JsonConvert.SerializeObject(new { toast = $"{room.Title} deleted successfully." })
            );
        }
    }
}
