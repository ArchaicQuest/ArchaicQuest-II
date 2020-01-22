using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    class Book
    {
        public int PageCount { get; set; }
        public List<string> Pages { get; set; }
        public bool Blank { get; set; }
    }
}
