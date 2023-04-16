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
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Model;
using Newtonsoft.Json;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.API.World
{
    [Authorize]
    public class RoomController : Controller
    {
        [HttpPost]
        [Route("api/World/Room")]
        public IActionResult Post([FromBody] Room room)
        {
            if (room == null)
            {
                return null;
            }
            
            var newRoom = room.MapRoom();
        
            GameLogic.Core.Services.Instance.DataBase.Save(newRoom, DataBase.Collections.Room);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            GameLogic.Core.Services.Instance.DataBase.Save(user, DataBase.Collections.Users);

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
            return GameLogic.Core.Services.Instance.DataBase.GetById<Room>(
                id,
                DataBase.Collections.Room
            );
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

            if (data == null)
            {
                return;
            }
            
            var updateRoom = data.MapRoom();
            GameLogic.Core.Services.Instance.DataBase.Save(updateRoom, DataBase.Collections.Room);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            GameLogic.Core.Services.Instance.DataBase.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail =
                    $"({data.AreaId}, {data.Id}, x: {data.Coords.X} y: {data.Coords.Y}, z: {data.Coords.Z}) {data.Title}",
                Type = DataBase.Collections.Room,
                UserName = user.Username
            };
            GameLogic.Core.Services.Instance.DataBase.Save(log, DataBase.Collections.Log);
        }

        [HttpGet("{x}/{y}/{z}/{areaId}")]
        [Route("api/World/Room/{x:int}/{y:int}/{z:int}/{areaId:int}")]
        public bool validExit(int x, int y, int z, int areaId)
        {
            return AddRoom.GetRoomFromCoords(
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
                //TEMP Fix
                if (mob.ClassName == "Thief") {
                    mob.ClassName = "Rogue";
                }
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

            var roomsWithPlayers = GameLogic.Core.Services.Instance.Cache
                .GetAllRooms()
                .Where(x => x.Players.Any());

            GameLogic.Core.Services.Instance.Cache.ClearRoomCache();

            try
            {
                var rooms = GameLogic.Core.Services.Instance.DataBase.GetList<Room>(
                    DataBase.Collections.Room
                );

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

                    GameLogic.Core.Services.Instance.Cache.AddRoom(
                        $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                        room
                    );
                    GameLogic.Core.Services.Instance.Cache.AddOriginalRoom(
                        $"{room.AreaId}{room.Coords.X}{room.Coords.Y}{room.Coords.Z}",
                        JsonConvert.DeserializeObject<Room>(JsonConvert.SerializeObject(room))
                    );
                }

                var areas = GameLogic.Core.Services.Instance.DataBase.GetList<Area>(
                    DataBase.Collections.Area
                );

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

                        GameLogic.Core.Services.Instance.Cache.AddMap(
                            $"{area.Id}{zarea.Coords.Z}",
                            Map.DrawMap(roomsByZ)
                        );
                    }

                    var rooms0index = roomList.FindAll(x => x.Coords.Z == 0);
                    GameLogic.Core.Services.Instance.Cache.AddMap(
                        $"{area.Id}0",
                        Map.DrawMap(rooms0index)
                    );
                }

                var helps = GameLogic.Core.Services.Instance.DataBase.GetList<Help>(
                    DataBase.Collections.Help
                );

                foreach (var help in helps)
                {
                    GameLogic.Core.Services.Instance.Cache.AddHelp(help.Id, help);
                }

                var quests = GameLogic.Core.Services.Instance.DataBase.GetList<Quest>(
                    DataBase.Collections.Quests
                );

                foreach (var quest in quests)
                {
                    GameLogic.Core.Services.Instance.Cache.AddQuest(quest.Id, quest);
                }

                var skills = GameLogic.Core.Services.Instance.DataBase.GetList<Skill>(
                    DataBase.Collections.Skill
                );

                foreach (var skill in skills)
                {
                    GameLogic.Core.Services.Instance.Cache.AddSkill(skill.Id, skill);
                }

                var craftingRecipes =
                    GameLogic.Core.Services.Instance.DataBase.GetList<CraftingRecipes>(
                        DataBase.Collections.CraftingRecipes
                    );
                foreach (var craftingRecipe in craftingRecipes)
                {
                    GameLogic.Core.Services.Instance.Cache.AddCraftingRecipes(
                        craftingRecipe.Id,
                        craftingRecipe
                    );
                }
                
                GameLogic.Core.Services.Instance.Writer.WriteLineAll("The winds of change have blown through the land, leaving behind new challenges and treasures to be discovered.");
            }
            catch (Exception ex)
            {
                
            }

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
            var room = GameLogic.Core.Services.Instance.DataBase
                .GetCollection<Room>(DataBase.Collections.Room)
                .FindById(id);
            GameLogic.Core.Services.Instance.DataBase.Delete<Room>(id, DataBase.Collections.Room);

            return Ok(
                JsonConvert.SerializeObject(new { toast = $"{room.Title} deleted successfully." })
            );
        }
        
        [HttpGet]
        [Route("api/World/Room/ReturnFlagTypes")]
        public JsonResult ReturnFlagTypes()
        {
            var roomFlags = new List<object>();

            foreach (var roomFlag in Enum.GetValues(typeof(Room.RoomFlag)))
            {
                roomFlags.Add(new { id = (int)roomFlag, name = roomFlag.ToString() });
            }
            return Json(roomFlags);
        }
    }
}
