using System;
using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.Engine.Character.Race.Commands;
using ArchaicQuestII.Engine.Character.Race.Model;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Character
{
    public class RaceController
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
            var query = new GetRacesQuery();
            return query.GetRaces();
        }
    }
}
