﻿using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Character.Model
{
    public class Affects
    {
        public bool NonDectect { get; set; } = false;
        public bool Invis { get; set; } = false;
        public bool DetectInvis { get; set; } = false;
        public bool Hidden { get; set; } = false;
        public bool DetectHidden { get; set; } = false;
        public bool Poisoned { get; set; } = false;
        public List<Affect> Custom { get; set; } = new List<Affect>();

    }

    public class Affect
    {
        public string Name { get; set; }
        public string Benefits { get; set; }
        public Modifier Modifier { get; set; }
        public int Duration { get; set; }
    }

}
