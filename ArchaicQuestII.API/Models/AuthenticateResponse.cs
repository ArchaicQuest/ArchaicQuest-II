using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;

namespace ArchaicQuestII.API.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }


        public AuthenticateResponse(AdminUser user, string token)
        {
            Id = user.Id;
            Username = user.Username;
            Token = token;
            Role = user.Role;
        }
    }
}
