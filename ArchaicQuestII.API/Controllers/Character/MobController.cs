using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.Item;
using Microsoft.Azure.KeyVault.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class MobController : Controller
    {

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
            };


            if (!string.IsNullOrEmpty(mob.Id.ToString()) && mob.Id != -1)
            {

                var foundItem = DB.FindById<Character>(mob.Id.ToString(), "Mobs");

                if (foundItem == null)
                {
                    throw new Exception("mob Id does not exist");
                }

                newMob.Id = mob.Id;
            }



            DB.Save(newMob, "Mobs");

        }


        //[HttpGet]
        //[Route("api/mob/Get")]
        //public List<Character> GetMob()
        //{

        //    var mobs = DB.GetItems();

        //    return mobs;

        //}


        [HttpGet]
        [Route("api/Character/Mob")]
        public List<Character> Get([FromQuery] string query)
        {

            var mobs =  DB.GetCollection<Character>("Mobs").Where(x => x.Name != null);

            if (string.IsNullOrEmpty(query))
            {
                return mobs.ToList();
            }

            return mobs.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        //[HttpGet]
        //[Route("api/mob/FindMobById")]
        //public Character FindMobById([FromQuery] int id)
        //{

        //    return DB.GetItems().FirstOrDefault(x => x.Id.Equals(id));

        //}




    }
}
