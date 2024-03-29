﻿using System;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Race;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace ArchaicQuestII.API.Controllers.Character
{

    [ApiController]
    public class RaceController : Controller
    {

        private IDataBase _db { get; }
        public RaceController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Helpers.Authorize]
        [Route("api/Character/Race")]
        public void Post(Race race)
        {
            _db.Save(race, DataBase.Collections.Race);

            var user = (HttpContext.Items["User"] as AdminUser);
            user.Contributions += 1;
            _db.Save(user, DataBase.Collections.Users);

            var log = new AdminLog()
            {
                Detail = $"({race.Id}) {race.Name}",
                Type = DataBase.Collections.Race,
                UserName = user.Username
            };
            _db.Save(log, DataBase.Collections.Log);
        }

        [HttpGet]
        [Helpers.Authorize]
        [Route("api/Character/Race/{id:int}")]
        public Race Get(int id)
        {
            return _db.GetById<Race>(id, DataBase.Collections.Race);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/Character/Race")]
        public List<Race> Get()
        {
            return _db.GetList<Race>(DataBase.Collections.Race);

        }
    }
}
