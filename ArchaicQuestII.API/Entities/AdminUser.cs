using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArchaicQuestII.API.Entities
{
    public class AdminUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime Joined { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public int Contributions { get; set; }

    }

    public class AddAdminUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }

    public class EditAdminUser: AddAdminUser
    {
        public int Id { get; set; }
    }
}
