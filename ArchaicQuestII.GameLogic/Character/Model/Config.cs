using System;

namespace ArchaicQuestII.GameLogic.Character.Model
{
    public class Config
    {
        public bool CanFollow { get; set; } = true;
        public bool NewbieChannel { get; set; } = true;
        public bool GossipChannel { get; set; } = true;
        public bool OocChannel { get; set; } = true;
        /// <summary>
        /// Don't show description
        /// </summary>
        public bool Brief { get; set; } = true;
        public bool AutoLoot { get; set; } = true;
        /// <summary>
        /// Splits gold in group combat
        /// </summary>
        public bool AutoSplit { get; set; } = true;
        public bool AutoSacrifice { get; set; } = false;
        public bool AutoAssist { get; set; } = false;
        public int GameFontSize { get; set; } = 16; //
        public int GameFont { get; set; } = 16; //
    }
}
