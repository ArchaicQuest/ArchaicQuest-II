using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Character.Class.Commands;
using ArchaicQuestII.Core.Character.Class.Model;
using ArchaicQuestII.Core.Character.Class.Queries;
using ArchaicQuestII.Core.Character.Race.Commands;
using ArchaicQuestII.Core.Character.Race.Model;
using ArchaicQuestII.Core.Item;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.Controllers.API.Character
{
    public class ClassController
    {

        [HttpPost]
        [Route("api/Character/Class")]
        public void Post(Class charClass)
        {
            var command = new CreateClassCommand();
            command.CreateClass(charClass);
        }

        [HttpGet]
        [Route("api/Character/Class/{id:int}")]
        public Class Get(int id)
        {
            var query = new GetClassQuery();
            return query.GetClass(id);
        }

        [HttpGet]
        [Route("api/Character/Class")]
        public List<Class> Get()
        {
            var query = new GetClassesQuery();
            return query.GetClasses();
        }
    }
}
