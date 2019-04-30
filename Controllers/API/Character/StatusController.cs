using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Character.Class.Queries;
using ArchaicQuestII.Core.Character.Race.Commands;
using ArchaicQuestII.Core.Character.Race.Model;
using ArchaicQuestII.Core.Item;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.Controllers.API.Character
{
    public class StatusController
    {

        [HttpPost]
        [Route("api/Character/Status")]
        public void Post(Option status)
        {
            var command = new CreateStatusCommand();
            command.CreateStatus(status);
        }

        [HttpGet]
        [Route("api/Character/Status/{id:int}")]
        public Option Get(int id)
        {
            var query = new GetStatusQuery();
            return query.GetStatus(id);
        }

        [HttpGet]
        [Route("api/Character/Status")]
        public List<Option> Get()
        {
            var query = new GetStatusesQuery();
            return query.GetStatuses();
        }
    }
}
