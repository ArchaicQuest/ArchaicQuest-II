using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Character.Class.Model;
using ArchaicQuestII.Engine.Character.Class.Queries;
using ArchaicQuestII.Engine.Character.Race.Commands;
using ArchaicQuestII.Engine.Character.Race.Model;
using ArchaicQuestII.Engine.Item;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Character
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
