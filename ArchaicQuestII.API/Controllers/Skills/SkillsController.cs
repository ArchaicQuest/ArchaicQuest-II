using ArchaicQuestII.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.GameLogic.Skill.Model;

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
