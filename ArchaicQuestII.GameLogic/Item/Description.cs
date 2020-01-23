using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Description
    {
        public string Taste { get; set; } = "It doesn't taste like much.";
        public string Touch { get; set; } = "It doesn't feel like much.";
        public string Smell { get; set; } = "It doesn't smell like much.";
        public string Look { get; set; } = "You see a {name}";
        public string Exam { get; set; } = "You don't see anything special.";
        public string Room { get; set; } = "{name} lies here.";
    }
}
