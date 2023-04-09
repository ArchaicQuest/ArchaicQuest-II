using ArchaicQuestII.DataAccess;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using ArchaicQuestII.GameLogic.Character.Class;

namespace ArchaicQuestII.API.Character
{

    public class ClassController : Controller
    {

        private IDataBase _db { get; }
        public ClassController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Helpers.Authorize]
        [Route("api/Character/Class")]
        public void Post([FromBody] IClass charClass)
        {

        }

        [HttpGet]
        [Helpers.Authorize]
        [Route("api/Character/Class/{id}")]
        public IClass Get(string id)
        {
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Character/Class")]
        public List<IClass> Get()
        {
            return null;
        }

    }
}
