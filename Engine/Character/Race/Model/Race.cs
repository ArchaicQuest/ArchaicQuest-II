﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Character.Model;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Race.Model
{
    public class Race: OptionDescriptive
    {
        [BsonField("p")]
        public bool Playable { get; set; }
        [BsonField("a")]
        public Attributes Attributes { get; set; } = new Attributes();
    }
}