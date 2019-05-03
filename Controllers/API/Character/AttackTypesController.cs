using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Race.Commands;
using ArchaicQuestII.Engine.Character.Race.Model;
using ArchaicQuestII.Engine.Item;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.Controllers.API.Character
{
    public class RaceController
    {

        [HttpPost]
        [Route("api/Character/Race")]
        public void Post(Race race)
        {
            var command = new CreateRaceCommand();
            command.CreateRace(race);
        }

        [HttpGet]
        [Route("api/Character/Race/{id:int}")]
        public Race Get(int id)
        {
            var query = new GetRaceQuery();
            return query.GetRace(id);
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
