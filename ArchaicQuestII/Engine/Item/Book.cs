using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Item
{
    public class Book
    {
        public int PageCount { get; set; }
        public List<string> Pages { get; set; }
        public bool Blank { get; set; }
    }
}
