using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.API.Models;
using ArchaicQuestII.API.Services;
using ArchaicQuestII.DataAccess.DataModels;
using ArchaicQuestII.GameLogic.Character.Help;
using Newtonsoft.Json;
using Account = ArchaicQuestII.GameLogic.Account.Account;
using AccountStats = ArchaicQuestII.GameLogic.Account.AccountStats;


namespace ArchaicQuestII.API.Controllers
{
    public class HelpController : Controller
    {
        private IDataBase _db { get; }
        private readonly IUserService _userService;
        public HelpController(IDataBase db, IUserService adminService)
        {
            _db = db;
            _userService = adminService;
        }

        [HttpPost]
        [Route("api/Help")]
        public IActionResult Post([FromBody] Help help)
        {

            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Help file");
                throw exception;
            }


            var data = new Help()
            {
                Description = help.Description,
               BriefDescription = help.BriefDescription,
               DateCreated = DateTime.Now,
               DateUpdated = DateTime.Now,
               Keywords = help.Keywords,
               RelatedHelpFiles = help.RelatedHelpFiles,
               Title = help.Title
            };

            if (!string.IsNullOrEmpty(help.Id.ToString()) && help.Id != -1)
            {

                var foundItem = _db.GetById<Help>(help.Id, DataBase.Collections.Skill);

                if (foundItem == null)
                {
                    throw new Exception("Help Id does not exist");
                }

                data.Id = help.Id;

            }

            var saved = _db.Save(data, DataBase.Collections.Help);

            string json = JsonConvert.SerializeObject(new { toast = "Help File created successfully", id = data.Id });
            return saved ? (IActionResult)Ok(json) : BadRequest("Error saving help");
        }

        [HttpGet]
        [Route("api/help/GetHelpById/{id:int}")]
        public Help GetHelpById(int id)
        {
            return _db.GetById<Help>(id, DataBase.Collections.Help);
        }


        [HttpGet]
        [Route("api/help")]
        public List<Help> GetHelp()
        {

            return _db.GetList<Help>(DataBase.Collections.Help).Where(x => x.Deleted.Equals(false)).ToList();

        }

        [HttpDelete]
        [Route("api/help/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var help = _db.GetById<Help>(id, DataBase.Collections.Help);
            help.Deleted = true;
            var saved = _db.Save(help, DataBase.Collections.Help);

            if (saved)
            {
                return Ok(JsonConvert.SerializeObject(new { toast = $"{help.Title} deleted successfully." }));
            }
            return Ok(JsonConvert.SerializeObject(new { toast = $"{help.Title} deletion failed." }));

        }



    }



}
