using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Book
    {
        /// <summary>
        /// Max allowed page count
        /// </summary>
        public int PageCount { get; set; }
        public List<string> Pages { get; set; }
        public bool Blank { get; set; }
    }
}
