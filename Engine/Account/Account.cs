using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Account
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Give insentive to verify
        /// allows us to email players and keep the playing or come back
        /// </summary>
        public bool EmailVerified { get; set; }
        public AccountStats Stats { get; set; } = new AccountStats();
        /// <summary>
        /// Characters associated with account
        /// </summary>
        public List<Character.Model.Player> Characters { get; set; } = new List<Character.Model.Player>();
        public DateTime DateJoined { get; set; } = DateTime.Now;
        public int Credits { get; set; }

    }
}
