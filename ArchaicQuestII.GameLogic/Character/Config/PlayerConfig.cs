using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Config
{
    public class PlayerConfig
    {
        /// <summary>
        /// Displays room name next to exit
        /// </summary>
        public bool VerboseExits { get; set; } = true; 
        /// <summary>
        /// Allows followers
        /// </summary>
        public bool CanFollow { get; set; } = true;
        /// <summary>
        /// Toggle newbie channel on/off
        /// </summary>
        public bool NewbieChannel { get; set; } = true;
        /// <summary>
        /// Toggle goddip channel on/off
        /// </summary>
        public bool GossipChannel { get; set; } = true;
        /// <summary>
        /// Toggle ooc channel on/off
        /// </summary>
        public bool OocChannel { get; set; } = true;
        /// <summary>
        /// Don't show description
        /// </summary>
        public bool Brief { get; set; } = false;
        /// <summary>
        /// Auto loot corpses
        /// </summary>
        public bool AutoLoot { get; set; } = true;
        /// <summary>
        /// Splits gold in group combat
        /// </summary>
        public bool AutoSplit { get; set; } = true;
        /// <summary>
        /// Sacrifice corpse
        /// </summary>
        public bool AutoSacrifice { get; set; } = false;
        /// <summary>
        /// Can receive messages via tell
        /// </summary>
        public bool Tells { get; set; } = true;
        /// <summary>
        /// Assist in combat
        /// </summary>
        public bool AutoAssist { get; set; } = false;
        public bool Hints { get; set; } = true;
        public int GameFontSize { get; set; } = 16; //
        public string GameFont { get; set; } = "Open Sans"; //
    }
}
