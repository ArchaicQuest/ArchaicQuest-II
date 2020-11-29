using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using Microsoft.AspNetCore.Mvc;

using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    [Authorize]
    public class MobController : Controller
    {

        private IDataBase _db { get; }
        public MobController(IDataBase db)
        {
            _db = db;
        }


        [HttpPost]
        [Route("api/Character/Mob")]
        public IActionResult Post([FromBody] Player mob)
           {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid mob");
                throw exception;
            }

            var newMob = new Player()
            {
                Name = mob.Name,
                LongName = mob.LongName,
                Status = mob.Status,
                Level = mob.Level,
                ArmorRating = new ArmourRating()
                {
                    Armour = mob.ArmorRating.Armour,
                    Magic = mob.ArmorRating.Magic
                },
                Affects = mob.Affects,
                AlignmentScore = mob.AlignmentScore,
                Attributes =  mob.Attributes,
                MaxAttributes = mob.Attributes,
                Inventory = mob.Inventory,
                Equipped = mob.Equipped,
                ClassName = mob.ClassName,
                Config = null,
                Description = mob.Description,
                Gender = mob.Gender,
                Stats = mob.Stats,
                MaxStats = mob.Stats,
                Money = mob.Money,
                Race = mob.Race,
                DefaultAttack = mob.DefaultAttack,
                DateCreated = mob.DateCreated ?? DateTime.Now,
                DateUpdated = DateTime.Now,
                Emotes = mob.Emotes,
                Commands = mob.Commands,
                Events = mob.Events,
                Roam = mob.Roam
            };


            if (mob.Id != Guid.Empty)
            {

                var foundItem = _db.GetById<Player>(mob.Id, DataBase.Collections.Mobs);

                if (foundItem == null)
                {
                    throw new Exception("mob Id does not exist");
                }

                newMob.Id = mob.Id;


                // If you update a mob, Update all the objects where it exists in a room
                // save yourself alot of work huh
                var rooms = _db.GetList<Room>(DataBase.Collections.Room);

                foreach (var room in rooms)
                {
                    foreach (var roomMob in room.Mobs.ToList())
                    {
                       if(roomMob.Id.Equals(newMob.Id)) {
                           room.Mobs[room.Mobs.FindIndex(x => x.Id.Equals(newMob.Id))] = newMob;
                       }
                    }
                   
                    _db.Save(room, DataBase.Collections.Room);
                }
            }


            _db.Save(newMob, DataBase.Collections.Mobs);
            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({newMob.Id}) {newMob.Name}",
                Type = DataBase.Collections.Mobs,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);

            return Ok(JsonConvert.SerializeObject(new { toast = $"Mob saved successfully." }));
        }


        [HttpGet]
        [Route("api/mob/Get")]
        public List<Player> GetMob()
        {

            var mobs = _db.GetCollection<Player>(DataBase.Collections.Mobs).FindAll().Where(x => x.Deleted == false).ToList();

            return mobs;

        }


        [HttpGet]
        [Route("api/Character/Mob")]
        public List<Player> Get([FromQuery] string query)
        {

            var mobs =  _db.GetCollection<Player>(DataBase.Collections.Mobs).FindAll().Where(x => x.Name != null && x.Deleted == false);

            if (string.IsNullOrEmpty(query))
            {
                return mobs.ToList();
            }

            return mobs.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        [HttpGet]
        [Route("api/mob/FindMobById")]
        public Player FindMobById([FromQuery] Guid id)
        {

            return _db.GetById<Player>(id, DataBase.Collections.Mobs);

        }


        [HttpDelete]
        [Route("api/mob/delete/{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var item = _db.GetCollection<Player>(DataBase.Collections.Mobs).FindById(id);
            item.Deleted = true;
            var saved = _db.Save(item, DataBase.Collections.Mobs);

            if (saved)
            {
                return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deleted successfully." }));
            }
            return Ok(JsonConvert.SerializeObject(new { toast = $"{item.Name} deletion failed." }));



        }



    }
}
