using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Character
{
    public class StatusController
    {

        private IDataBase _db { get; }
        public StatusController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Character/Status")]
        public void Post(Option status)
        {
            _db.Save(status, DataBase.Collections.Status);
        }

        [HttpGet]
        [Route("api/Character/Status/{id:int}")]
        public Option Get(int id)
        {
            return _db.GetById<Option>(id, DataBase.Collections.Status);
        }

        [HttpGet]
        [Route("api/Character/Status")]
        public List<Option> Get()
        {
            return _db.GetList<Option>(DataBase.Collections.Status);
        }
    }
}
