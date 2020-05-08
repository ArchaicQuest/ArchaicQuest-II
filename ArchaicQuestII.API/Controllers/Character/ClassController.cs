using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Class;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ArchaicQuestII.API.Character
{
    public class ClassController
    {

        private IDataBase _db { get; }
        public ClassController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Character/Class")]
        public void Post(Class charClass)
        {
            _db.Save(charClass, DataBase.Collections.Class);
        }

        [HttpGet]
        [Route("api/Character/Class/{id:int}")]
        public Class Get(int id)
        {
           return _db.GetById<Class>(id, DataBase.Collections.Class);
        }

        [HttpGet]
        [Route("api/Character/Class")]
        public List<Class> Get()
        {
            return _db.GetList<Class>(DataBase.Collections.Class);
        }

    }
}
