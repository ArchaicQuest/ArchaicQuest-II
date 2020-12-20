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
using Newtonsoft.Json;
using Account = ArchaicQuestII.GameLogic.Account.Account;
using AccountStats = ArchaicQuestII.GameLogic.Account.AccountStats;


namespace ArchaicQuestII.API.Controllers
{
    public class AccountController : Controller
    {
        private IDataBase _db { get; }
        private readonly IUserService _userService;
        public AccountController(IDataBase db, IUserService adminService)
        {
            _db = db;
            _userService = adminService;
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

        [HttpPost]
        [Route("api/Account/Profile")]
        public IActionResult GetProfile([FromBody] Guid id)
        {

            if (!ModelState.IsValid)
            {
                var exception = new Exception("Invalid request");
                throw exception;
            }

            var user = _db.GetCollection<Account>(DataBase.Collections.Account).FindOne(x => x.Id.Equals(id));

            if (user == null)
            {
                return BadRequest("Sorry that account does not exist.");
            }

            var characters = _db.GetCollection<Player>(DataBase.Collections.Players)
                .Find(x => x.AccountId.Equals(user.Id));

            var profile = new AccountViewModel()
            {
                Characters = characters.ToList(),
                Credits = 0,
                DateJoined = user.DateJoined,
                Stats = new AccountStats()
                {

                }
            };


            return Ok(JsonConvert.SerializeObject(new { toast = "logged in successfully", profile }));

        }


        //https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
        [HttpPost("api/Account/authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest userParam)
        {
            var response = _userService.Authenticate(userParam);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpPost("api/Account/adduser")]
        public IActionResult Add([FromBody] AddAdminUser user)
        {
            var userExists = _db.GetList<AdminUser>(DataBase.Collections.Users).FirstOrDefault(x => x.Username.Equals(user.Username, StringComparison.CurrentCultureIgnoreCase));

            var context = (HttpContext.Items["User"] as AdminUser);
            if (context.Role != Role.Admin)
            {
                return BadRequest(new { message = "Only admin can add accounts." });
            }

            if (userExists != null)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var adminUser = new AdminUser()
            {
                Username = user.Username,
                Password = user.Password,
                Role = user.Role
            };
            _db.Save(adminUser, DataBase.Collections.Users);


            return Ok(new { message = "User successfully added" });
        }

        [Authorize]
        [HttpPost("api/Account/edituser")]
        public IActionResult Edit([FromBody] EditAdminUser user)
        {
            var userExists = _db.GetById<AdminUser>(user.Id, DataBase.Collections.Users);

            if (userExists == null)
            {
                return BadRequest(new { message = "User does not exists." });
            }

            var adminUser = new AdminUser()
            {
                Id = user.Id,
                Username = user.Username,
                Password = string.IsNullOrEmpty(user.Password) ? userExists.Password : user.Password,
                Role = user.Role,
                LastActive = DateTime.Now
            };
            _db.Save(adminUser, DataBase.Collections.Users);


            return Ok(new { message = "User successfully updated" });
        }

        [Authorize]
        [HttpPost("api/Account/deleteUser")]
        public IActionResult Delete([FromBody] int id)
        {
            var userExists = _db.GetById<AdminUser>(id, DataBase.Collections.Users);

            if (userExists == null)
            {
                return BadRequest(new { message = "User does not exists." });
            }

           
            if ((HttpContext.Items["User"] as AdminUser).Role != Role.Admin)
            {
                return BadRequest(new { message = "You need to be admin to do this" });
            }

           
            var deleted = _db.Delete<AdminUser>(userExists.Id, DataBase.Collections.Users);

            if (deleted)
            {
                return Ok(new { message = "User deleted successfully" });
            }

            return BadRequest(new { message = "User deletion failed" });

        }




        [Authorize]
        [HttpGet("api/Account/getusers")]
        public IActionResult GetAll()
        {
          
            
            var users = _userService.GetAll();
            var context = (HttpContext.Items["User"] as AdminUser);
            foreach (var user in users)
            {
                if (user.Username == "Admin" || (context.Role == Role.Admin))
                {
                    user.CanDelete = true;
                    user.CanEdit = true;
                }

                if (context.Id.Equals(user.Id))
                {
                    user.CanDelete = false;
                    user.CanEdit = true;
                }
            }

            return Ok(users);
        }

        [Authorize]
        [HttpGet("api/Account/logs")]
        public IActionResult GetLogs()
        {

            var logs = _db.GetList<AdminLog>(DataBase.Collections.Log);
            return Ok(logs);
        }
    }



}
