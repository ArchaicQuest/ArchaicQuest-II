using ArchaicQuestII.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using System.Linq;


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
