using System;
using ArchaicQuestII.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Models;
using Microsoft.AspNetCore.Authorization;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Core;

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
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid object");
                throw exception;
            }

            IClass newClass = CoreHandler.Instance.CharacterHandler.GetClass(charClass.Name);

            _db.Save(newClass, DataBase.Collections.Class);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({newClass.Id}) {newClass.Name}",
                Type = DataBase.Collections.Class,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
        }

        [HttpGet]
        [Helpers.Authorize]
        [Route("api/Character/Class/{id}")]
        public IClass Get(string id)
        {
            return CoreHandler.Instance.CharacterHandler.GetClass(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Character/Class")]
        public List<IClass> Get()
        {
            return CoreHandler.Instance.CharacterHandler.GetClasses(false);
        }
    }
}
