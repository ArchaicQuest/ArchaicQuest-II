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
            return saved ? (IActionResult)Ok(json) : BadRequest("Error saving account");
        }

        [HttpPost]
        [Route("api/Account/Login")]
        public IActionResult Login([FromBody] Login login)
        {

            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid login details");
                throw exception;
            }

            var user = _db.GetCollection<Account>(DataBase.Collections.Account).FindOne(x => x.Email.Equals(login.Username));

            if (user == null)
            {
                return BadRequest("Sorry that account does not exist.");
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return BadRequest("Password is not correct.");
            }      

            return (IActionResult)Ok(JsonConvert.SerializeObject(new { toast = "logged in successfully", id = user.Id }));
            
        }
    }



}
