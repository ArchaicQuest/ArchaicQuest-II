using System;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Race;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ArchaicQuestII.API.Controllers.Character
{
    public class RaceController : Controller
    {

        private IDataBase _db { get; }
        public RaceController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Character/Race")]
        public void Post(Race race)
        {
            _db.Save(race, DataBase.Collections.Race);
        }

        [HttpGet]
        [Route("api/Character/Race/{id:int}")]
        public Race Get(int id)
        {
            return _db.GetById<Race>(id, DataBase.Collections.Race);
        }

        [HttpGet]
        [Route("api/Character/Race")]
        public List<Race> Get()
        { 
            return _db.GetList<Race>(DataBase.Collections.Race);
          
        }
    }
}
