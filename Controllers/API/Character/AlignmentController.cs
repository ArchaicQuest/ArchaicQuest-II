using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Character.Class.Queries;
using ArchaicQuestII.Core.Character.Race.Commands;
using ArchaicQuestII.Core.Character.Race.Model;
using ArchaicQuestII.Core.Item;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.Controllers.API.Character
{
    public class AlignmentController
    {

        [HttpPost]
        [Route("api/Character/Alignment")]
        public void Post(Alignment align)
        {
            var command = new CreateAlignmentCommand();
            command.CreateAlignment(align);
        }

        [HttpGet]
        [Route("api/Character/Alignment/{id:int}")]
        public Alignment Get(int id)
        {
            var query = new GetAlignmentQuery();
            return query.GetAlignment(id);
        }

        [HttpGet]
        [Route("api/Character/Alignment")]
        public List<Alignment> Get()
        {
            var query = new GetAlignmentsQuery();
            return query.GetAlignments();
        }
    }
}
