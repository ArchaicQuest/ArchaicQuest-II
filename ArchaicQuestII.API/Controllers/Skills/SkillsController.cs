using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Skill.Model;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.Skills
{
    [Authorize]
    public class SkillsController : Controller
    {
        private IDataBase _db { get; }
        public SkillsController(IDataBase db)
        {
            _db = db;
        }
       
        
        [HttpGet]
        [Route("api/skill/Get")]
        public List<Skill> GetSkill()
        {

            return _db.GetList<Skill>(DataBase.Collections.Skill).ToList();

        }


        [HttpGet]
        [Route("api/skill/FindSkillById")]
        public Skill FindSkillById([FromQuery] int id)
        {

            return _db.GetCollection<Skill>(DataBase.Collections.Skill).FindById(id);

        }


    }
}
