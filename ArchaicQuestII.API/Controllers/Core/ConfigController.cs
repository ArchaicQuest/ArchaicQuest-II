using System;
using System.Net;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.AspNetCore.Mvc;

namespace ArchaicQuestII.API.Controllers.Core;

[Authorize]
public class ConfigController : Controller
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

        if (config == null) { return HttpStatusCode.BadRequest; }
        _db.Save(config, DataBase.Collections.Config);
        return HttpStatusCode.OK;

    }

    [HttpGet]
    [Route("api/config")]
    public Config Get()
    {
        var config = _db.GetById<Config>(1, DataBase.Collections.Config);
        return config;
    }
}