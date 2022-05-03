using System;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Class;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Authorization;

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
        public void Post([FromBody] Class charClass)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid object");
                throw exception;
            }

            var newClass = new Class()
            {
                Name = charClass.Name,
                AttributeBonus = charClass.AttributeBonus,
                DateCreated = charClass.Id == -1 ? DateTime.Now : charClass.DateCreated,
                DateUpdated = charClass.Id == -1 ? DateTime.Now : charClass.DateUpdated,
                CreatedBy = "Malleus",
                Description = charClass.Description,
                ExperiencePointsCost = charClass.ExperiencePointsCost,
                HitDice = new Dice()
                {
                    DiceMinSize = 1,
                    DiceMaxSize = charClass.HitDice.DiceMaxSize,
                    DiceRoll = 1
                },
                Skills = charClass.Skills
            };

            if (!string.IsNullOrEmpty(charClass.Id.ToString()) && charClass.Id != -1)
            {

                var foundClass = _db.GetById<Class>(charClass.Id, DataBase.Collections.Class);

                if (foundClass == null)
                {
                    throw new Exception("Item Id does not exist");
                }

                newClass.DateUpdated = DateTime.Now;
                newClass.Id = charClass.Id;
            }

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
        [Route("api/Character/Class/{id:int}")]
        public Class Get(int id)
        {
            return _db.GetById<Class>(id, DataBase.Collections.Class);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Character/Class")]
        public List<Class> Get()
        {
            return _db.GetList<Class>(DataBase.Collections.Class);
        }

    }
}
