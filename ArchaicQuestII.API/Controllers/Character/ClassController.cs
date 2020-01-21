using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Character.Class.Model;
using Microsoft.AspNetCore.Mvc;

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
