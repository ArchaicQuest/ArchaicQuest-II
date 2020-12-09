using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;

namespace ArchaicQuestII.API.Models
{
    public class AdminLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DataBase.Collections Type { get; set; }
        public string Detail { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
