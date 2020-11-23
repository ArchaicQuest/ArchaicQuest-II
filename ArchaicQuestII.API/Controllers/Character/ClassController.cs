using System;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Class;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.API.Character
{
    [Authorize]
    public class ClassController : Controller
    {

        private IDataBase _db { get; }
        public ClassController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
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
