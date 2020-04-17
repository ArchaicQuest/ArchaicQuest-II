using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.DataAccess;
using Microsoft.AspNetCore.Mvc;

using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Item;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class MobController : Controller
    {

        private IDataBase _db { get; }
        public MobController(IDataBase db)
        {
            _db = db;
        }


        [HttpPost]
        [Route("api/Character/Mob")]
        public void Post([FromBody] Character mob)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid mob");
                throw exception;
            }

            var newMob = new Character()
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
                Attributes = mob.Attributes,
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
                DefaultAttack = mob.DefaultAttack
            };


            if (mob.Id != Guid.Empty)
            {

                var foundItem = _db.GetById<Character>(mob.Id, DataBase.Collections.Mobs);

                if (foundItem == null)
                {
                    throw new Exception("mob Id does not exist");
                }

                newMob.Id = mob.Id;
            }


            _db.Save(newMob, DataBase.Collections.Mobs);

        }


        [HttpGet]
        [Route("api/mob/Get")]
        public List<Character> GetMob()
        {

            var mobs = _db.GetCollection<Character>(DataBase.Collections.Mobs).FindAll().ToList();

            return mobs;

        }


        [HttpGet]
        [Route("api/Character/Mob")]
        public List<Character> Get([FromQuery] string query)
        {

            var mobs =  _db.GetCollection<Character>(DataBase.Collections.Mobs).FindAll().Where(x => x.Name != null);

            if (string.IsNullOrEmpty(query))
            {
                return mobs.ToList();
            }

            return mobs.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        [HttpGet]
        [Route("api/mob/FindMobById")]
        public Character FindMobById([FromQuery] Guid id)
        {

            return _db.GetById<Character>(id, DataBase.Collections.Mobs);

        }




    }
}
