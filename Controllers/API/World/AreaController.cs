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
using ArchaicQuestII.Engine.World;
using ArchaicQuestII.Engine.World.Area.Commands;
using ArchaicQuestII.Engine.World.Area.Model;
using ArchaicQuestII.Engine.World.Area.Queries;
using Microsoft.Azure.KeyVault.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers
{
    public class AreaController : Controller
    {

        [HttpPost]
        [Route("api/World/Area")]
        public void Post([FromBody] Area area)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid area");
                throw exception;
            }

            var newArea = new Area()
            {
                Title = area.Title,
                Description = area.Description,
                DateCreated = DateTime.Now,
                CreatedBy = "Malleus",
                DateUpdated = DateTime.Now
           
             
            };


            if (!string.IsNullOrEmpty(area.Id.ToString()) && area.Id != -1)
            {

                var foundItem = DB.GetMob(area.Id.ToString());

                if (foundItem == null)
                {
                    throw new Exception("mob Id does not exist");
                }

                newArea.Id = area.Id;
            }



            DB.SaveArea(newArea);

        }

        [HttpGet]
        [Route("api/World/Area")]
        public List<Area> Get()
        {

           return DB.GetAreas();

        }

        [HttpGet]
        [Route("api/World/Area/{id:int}")]
        public Area Get(int id)
        {

            return new GetAreaQuery().GetArea(id);

        }

        [HttpPut]
        [Route("api/World/Area/{id:int}")]
        public void Put([FromBody] Area data)
        {

            new UpdateAreaCommand().UpdateArea(data);

        }

    }
}
