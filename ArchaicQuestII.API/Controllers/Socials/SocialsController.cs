using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Core
{
    [Authorize]
    public class SocialsController : Controller
    {
        private IDataBase _db { get; }
        public SocialsController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/socials")]
        public HttpStatusCode Post([FromBody] Emote social)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid settings");
                throw exception;
            }

            if (social == null) { return HttpStatusCode.BadRequest; }
            _db.Save(social, DataBase.Collections.Socials);
            return HttpStatusCode.OK;

        }

        // Does not work Litedb wont return the correct values for a dictionary
        [HttpGet]
        [Route("api/socials")]
        public List<KeyValuePair<string, Emote>> Get()
        {
            var x = _db.GetCollection<KeyValuePair<string, Emote>>(DataBase.Collections.Socials);
            var z = x.FindAll();
            return null;
        }

    }
}
