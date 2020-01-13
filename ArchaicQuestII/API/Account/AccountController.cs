using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
        public IActionResult Post([FromBody] Account account)
        {


            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid Account details");
                throw exception;
            }

            var hasEmail = DB.GetColumn<Account>("Account").FindOne(x => x.Email.Equals(account.Email));

            if (hasEmail != null)
            {
                return BadRequest("An account with that email address already exists.");
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


           var saved = DB.Save(data, "Account");

            return saved ? (IActionResult) Ok("account created successfully") : BadRequest("Error saving account");
        }

    }
}
