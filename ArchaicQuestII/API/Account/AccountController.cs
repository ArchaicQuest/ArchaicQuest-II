using System;
using System.Collections.Generic;
using ArchaicQuestII.Engine.Character.Model;
using Microsoft.AspNetCore.Mvc;
using ArchaicQuestII.Core.Events;
using ArchaicQuestII.Engine.Account;
 
namespace ArchaicQuestII.Controllers
{
    public class AccountController : Controller
    {

        [HttpPost]
        [Route("api/Account")]
        public void Post([FromBody] Account account)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Account details");
                throw exception;
            }


            var data = new Account()
            {
                UserName = account.UserName,
                Characters = new List<Player>(),
                Credits = 0,
                Email = account.Email,
                EmailVerified = false,
                Password = BCrypt.Net.BCrypt.HashPassword(account.Password), //BCrypt.Verify("my password", passwordHash);
                Stats = new AccountStats(),
            };


            DB.Save(data, "Account");

        }

    }
}
