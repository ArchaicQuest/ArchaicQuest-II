using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArchaicQuestII.API.Controllers
{
    public class AccountController : Controller
    {
        private IDataBase _db { get; }
        public AccountController(IDataBase db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("api/Account")]
        public IActionResult Post([FromBody] Account account)
        {

            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Account details");
                throw exception;
            }

            var hasEmail = _db.GetCollection<Account>(DataBase.Collections.Account).FindOne(x => x.Email.Equals(account.Email));

            if (hasEmail != null)
            {
                return BadRequest("An account with that email address already exists.");
            }

            var data = new Account()
            {
                UserName = account.UserName,
                Id = Guid.NewGuid(),
                Characters = new List<Guid>(),
                Credits = 0,
                Email = account.Email,
                EmailVerified = false,
                Password = BCrypt.Net.BCrypt.HashPassword(account.Password), //BCrypt.Verify("my password", passwordHash);
                Stats = new AccountStats(),
            };

            var saved = _db.Save(data, DataBase.Collections.Account);

            string json = JsonConvert.SerializeObject(new { toast = "account created successfully", id = data.Id });
            return saved ? (IActionResult) Ok(json) : BadRequest("Error saving account");}
        }

    }
