using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Core
{
    public class ConfigController: Controller
    {
        private IDataBase _db { get; }
        public ConfigController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/config")]
        public HttpStatusCode Post([FromBody] Config config)
        {
            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid settings");
                throw exception;
            }

            if (config == null) {return HttpStatusCode.BadRequest; }
            _db.Save(config, DataBase.Collections.Config);
            return HttpStatusCode.OK;

        }

        [HttpGet]
        [Route("api/config")]
        public Config Get()
        {
            return _db.GetById<Config>(1, DataBase.Collections.Config);
        }

    }
}
