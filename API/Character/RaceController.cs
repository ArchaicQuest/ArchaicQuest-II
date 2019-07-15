using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Class.Queries;
using ArchaicQuestII.Engine.Character.Race.Commands;
using ArchaicQuestII.Engine.Character.Race.Model;
using ArchaicQuestII.Engine.Item;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Character
{
    public class AttackTypesController
    {

        [HttpPost]
        [Route("api/Character/AttackType")]
        public void Post(OptionDescriptive attackType)
        {
            var command = new CreateAttackTypeCommand();
            command.CreateAttackType(attackType);
        }

        [HttpGet]
        [Route("api/Character/AttackType/{id:int}")]
        public OptionDescriptive Get(int id)
        {
            var query = new GetAttackTypeQuery();
            return query.GetAttackType(id);
        }

        [HttpGet]
        [Route("api/Character/AttackType")]
        public List<OptionDescriptive> Get()
        {
            var query = new GetAttackTypesQuery();
            return query.GetAttackTypes();
        }
    }
}
