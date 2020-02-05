using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Money
    {
        public int Amount { get; set; }

    }

    public class Gold : Money
    {
        private Description Description { get; set; } = new Description
        {
            Look = "A gold coin.",
            Exam = "A gold coin.",
            Room = "A gold coin."
        };
    }
}
