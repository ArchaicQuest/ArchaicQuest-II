﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Config
{
    public class PlayerConfig
    {
        /// <summary>
        /// Displays room name next to exit
        /// </summary>
        public bool VerboseExits { get; set; } = true; //
        public bool CanFollow { get; set; } = true;
        public bool NewbieChannel { get; set; } = true;
        public bool GossipChannel { get; set; } = true;
        public bool OocChannel { get; set; } = true;
        /// <summary>
        /// Don't show description
        /// </summary>
        public bool Brief { get; set; } = false; //
        public bool AutoLoot { get; set; } = true;
        /// <summary>
        /// Splits gold in group combat
        /// </summary>
        public bool AutoSplit { get; set; } = true;
        public bool AutoSacrifice { get; set; } = false;
        public bool AutoAssist { get; set; } = false;
        public int GameFontSize { get; set; } = 16; //
        public string GameFont { get; set; } = "Open Sans"; //
    }
}
