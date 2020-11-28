using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Core
{
    [Authorize]
    public class QuestController: Controller
    {
        private IDataBase _db { get; }
        public QuestController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Quest")]
        public HttpStatusCode Post([FromBody] Quest Quest)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Quest");
                throw exception;
            }

            if (Quest == null) {return HttpStatusCode.BadRequest; }

            var newQuest = new Quest()
            {
                Title = Quest.Title,
                Area = Quest.Area,
                Description = Quest.Description,
                ExpGain = Quest.ExpGain,
                GoldGain = Quest.GoldGain,
                ItemGain = Quest.ItemGain,
                MobsToKill = Quest.MobsToKill
            };

            if (Quest.Id != -1)
            {

                var foundItem = _db.GetById<Quest>(Quest.Id, DataBase.Collections.Quests);

                if (foundItem == null)
                {
                    throw new Exception("quest Id does not exist");
                }

                newQuest.Id = newQuest.Id;
            }

            _db.Save(newQuest, DataBase.Collections.Quests);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);
            return HttpStatusCode.OK;

        }

        [HttpGet]
        [Route("api/Quest")]
        public Quest Get(int id)
        {
            return _db.GetById<Quest>(id, DataBase.Collections.Quests);
        }

        [HttpGet]
        [Route("api/Quest/GetQuests")]
        public List<Quest> GetQuests()
        {

            return _db.GetList<Quest>(DataBase.Collections.Quests).ToList();

        }

    }
}
