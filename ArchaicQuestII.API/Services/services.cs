using ArchaicQuestII.API.Entities;
using ArchaicQuestII.API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArchaicQuestII.API.Services
{
    public class services
    {
        public interface IAdminUserService
        {
            AdminUser Authenticate(string AdminUsername, string password);
            IEnumerable<AdminUser> GetAll();
        }

        public class AdminUserService : IAdminUserService
        {
            // AdminUsers hardcoded for simplicity, store in a db with hashed passwords in production applications
            private List<AdminUser> _AdminUsers = new List<AdminUser>
            {
                new AdminUser {Id = 1, FirstName = "Test", LastName = "AdminUser", Username = "test", Password = "test"}
            };

            private readonly AppSettings _appSettings;

            public AdminUserService(IOptions<AppSettings> appSettings)
            {
                _appSettings = appSettings.Value;
            }

            public AdminUser Authenticate(string AdminUsername, string password)
            {
                var AdminUser = _AdminUsers.SingleOrDefault(x => x.Username == AdminUsername && x.Password == password);

                // return null if AdminUser not found
                if (AdminUser == null)
                    return null;

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, AdminUser.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                AdminUser.Token = tokenHandler.WriteToken(token);

                // remove password before returning
                AdminUser.Password = null;

                return AdminUser;
            }

            public IEnumerable<AdminUser> GetAll()
            {
                // return AdminUsers without passwords
                return _AdminUsers.Select(x =>
                {
                    x.Password = null;
                    return x;
                });
            }
        }
    }
}
