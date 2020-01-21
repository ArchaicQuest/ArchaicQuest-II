
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.Character.Equipment.Model;
using ArchaicQuestII.Engine.Character.Status;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Item;
using Microsoft.Azure.KeyVault.Models;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class PlayerController : Controller
    {

        [HttpPost]
        [Route("api/Character/Player")]
        public void Post([FromBody] Player player)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid player");
                throw exception;
            }
 

            var newPlayer = new Player()
            {
                Name = player.Name,
                Status = Status.Standing,
                Level = 1,
                ArmorRating = new ArmourRating()
                {
                    Armour = 1,
                    Magic = 1
                },
                Affects = new Affects(),
                AlignmentScore = 0,
                Attributes = player.Attributes,
                MaxAttributes = player.Attributes,
                Inventory = new List<Item>(),
                Equipped = new Equipment(),
                ClassName = player.ClassName,
                Config = null,
                Description = player.Description,
                Gender = player.Gender,
                Stats = new Stats(),
                MaxStats = player.Stats,
                Money = new Engine.Character.Model.Money()
                {
                    Gold = 100
                },
                Race = player.Race,
                JoinedDate = new DateTime()
                
            };


            if (!string.IsNullOrEmpty(player.Id.ToString()) && player.Id != -1)
            {

                var foundItem = DB.FindById<Character>(player.Id.ToString(), "players");

                if (foundItem == null)
                {
                    throw new Exception("player Id does not exist");
                }

                newPlayer.Id = player.Id;
            }



            DB.Save(newPlayer, "players");

        }


        //[HttpGet]
        //[Route("api/player/Get")]
        //public List<Character> GetMob()
        //{

        //    var mobs = DB.GetItems();

        //    return mobs;

        //}


        [HttpGet]
        [Route("api/Character/player")]
        public List<Character> Get([FromQuery] string query)
        {

            var mobs = DB.GetCollection<Character>("Mobs").Where(x => x.Name != null);



            if (string.IsNullOrEmpty(query))
            {
                return mobs.ToList();
            }

            return mobs.Where(x => x.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1).ToList();

        }

        //[HttpGet]
        //[Route("api/player/FindMobById")]
        //public Character FindMobById([FromQuery] int id)
        //{

        //    return DB.GetItems().FirstOrDefault(x => x.Id.Equals(id));

        //}




    }
}
