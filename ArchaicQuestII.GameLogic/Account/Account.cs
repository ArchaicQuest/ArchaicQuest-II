using ArchaicQuestII.GameLogic.Character;
using LiteDB;
using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Account
{
    public class Account
    {
        public Guid Id { get; set; }
        [BsonField("n")]
        public string UserName { get; set; }
        [BsonField("p")]
        public string Password { get; set; }
        [BsonField("e")]
        public string Email { get; set; }
        /// <summary>
        /// Give insentive to verify
        /// allows us to email players and keep the playing or come back
        /// </summary>
        [BsonField("ev")]
        public bool EmailVerified { get; set; }
        [BsonField("s")]
        public AccountStats Stats { get; set; } = new AccountStats();
        /// <summary>
        /// Characters associated with account
        /// </summary>
        [BsonField("c")]
        public List<Guid> Characters { get; set; } = new List<Guid>();
        [BsonField("dj")]
        public DateTime DateJoined { get; set; } = DateTime.Now;
        [BsonField("c")]
        public int Credits { get; set; }

    }
}
