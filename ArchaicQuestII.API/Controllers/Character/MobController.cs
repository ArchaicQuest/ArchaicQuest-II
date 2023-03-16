using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using Microsoft.AspNetCore.Mvc;

using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
public class MobData
{
    public Player Mob { get; set; }
    public bool UpdateAllInstances { get; set; }
}

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
        public IActionResult Post([FromBody] MobData mob)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid mob");
                throw exception;
            }

            var newMob = new Player()
            {
                Name = mob.Mob.Name,
                LongName = mob.Mob.LongName,
                Status = mob.Mob.Status,
                Level = mob.Mob.Level,
                ArmorRating = new ArmourRating()
                {
                    Armour = mob.Mob.ArmorRating.Armour,
                    Magic = mob.Mob.ArmorRating.Magic
                },
                Affects = mob.Mob.Affects,
                AlignmentScore = mob.Mob.AlignmentScore,
                Attributes = mob.Mob.Attributes,
                MaxAttributes = mob.Mob.Attributes,
                Inventory = mob.Mob.Inventory,
                Equipped = mob.Mob.Equipped,
                ClassName = mob.Mob.ClassName,
                Config = null,
                Description = mob.Mob.Description,
                Gender = mob.Mob.Gender,
                Stats = mob.Mob.Stats,
                MaxStats = mob.Mob.Stats,
                Money = mob.Mob.Money,
                Race = mob.Mob.Race,
                DefaultAttack = mob.Mob.DefaultAttack,
                DateCreated = mob.Mob.DateCreated ?? DateTime.Now,
                DateUpdated = DateTime.Now,
                Emotes = mob.Mob.Emotes,
                Commands = mob.Mob.Commands,
                Events = mob.Mob.Events,
                Aggro = mob.Mob.Aggro,
                Roam = mob.Mob.Roam,
                Shopkeeper = mob.Mob.Shopkeeper,
                Trainer = mob.Mob.Trainer,
                Mounted = mob.Mob.Mounted,
                SpellList = mob.Mob.SpellList,
                EnterEmote = mob.Mob.EnterEmote,
                LeaveEmote = mob.Mob.LeaveEmote,
                IsHiddenScriptMob = mob.Mob.IsHiddenScriptMob
            };

            foreach (var item in mob.Mob.Inventory)
            {
                if (item.Equipped)
                {
                    switch (item.Slot)
                    {
                        case Equipment.EqSlot.Arms:
                            mob.Mob.Equipped.Arms = item;
                            break;
                        case Equipment.EqSlot.Body:
                            mob.Mob.Equipped.AboutBody = item;
                            break;
                        case Equipment.EqSlot.Face:
                            mob.Mob.Equipped.Face = item;
                            break;
                        case Equipment.EqSlot.Feet:
                            mob.Mob.Equipped.Feet = item;
                            break;
                        case Equipment.EqSlot.Finger:
                            mob.Mob.Equipped.Finger = item;
                            break;
                        case Equipment.EqSlot.Floating:
                            mob.Mob.Equipped.Floating = item;
                            break;
                        case Equipment.EqSlot.Hands:
                            mob.Mob.Equipped.Hands = item;
                            break;
                        case Equipment.EqSlot.Head:
                            mob.Mob.Equipped.Head = item;
                            break;
                        case Equipment.EqSlot.Held:
                            mob.Mob.Equipped.Held = item;
                            break;
                        case Equipment.EqSlot.Legs:
                            mob.Mob.Equipped.Legs = item;
                            break;
                        case Equipment.EqSlot.Light:
                            mob.Mob.Equipped.Light = item;
                            break;
                        case Equipment.EqSlot.Neck:
                            mob.Mob.Equipped.Neck = item;
                            break;
                        case Equipment.EqSlot.Shield:
                            mob.Mob.Equipped.Shield = item;
                            break;
                        case Equipment.EqSlot.Torso:
                            mob.Mob.Equipped.Torso = item;
                            break;
                        case Equipment.EqSlot.Waist:
                            mob.Mob.Equipped.Waist = item;
                            break;
                        case Equipment.EqSlot.Wielded:
                            mob.Mob.Equipped.Wielded = item;

                            break;
                        case Equipment.EqSlot.Wrist:
                            mob.Mob.Equipped.Wrist = item;
                            break;
                        case Equipment.EqSlot.Secondary:
                            mob.Mob.Equipped.Secondary = item;
                            break;
                    }
                }
            }


            if (mob.Mob.Id != Guid.Empty)
            {

                var foundItem = _db.GetById<Player>(mob.Mob.Id, DataBase.Collections.Mobs);

                if (foundItem == null)
                {
                    throw new Exception("mob Id does not exist");
                }

                newMob.Id = mob.Mob.Id;


                // If you update a mob, Update all the objects where it exists in a room
                // save yourself alot of work huh

                if (mob.UpdateAllInstances)
                {

                    var rooms = _db.GetList<Room>(DataBase.Collections.Room);

                    foreach (var room in rooms)
                    {
                        foreach (var roomMob in room.Mobs.ToList())
                        {
                            if (roomMob.Id.Equals(newMob.Id))
                            {
                                newMob.UniqueId = roomMob.UniqueId;
                                room.Mobs.Remove(roomMob);
                                room.Mobs.Add(newMob);
                            }
                        }

                        _db.Save(room, DataBase.Collections.Room);
                    }
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

            var mobs = _db.GetCollection<Player>(DataBase.Collections.Mobs).FindAll().Where(x => x.Name != null && x.Deleted == false);

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
