using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class Modifier
    {
        /// <summary>
        /// Increases or decreases the characters hitroll score
        /// </summary>
        public int HitRoll { get; set; }
        /// <summary>
        /// Increases or decreases the characters damroll score
        /// </summary
        public int DamRoll { get; set; }
        /// <summary>
        /// Increases or decreases the characters save score
        /// </summary
        public int Saves { get; set; }
        /// <summary>
        /// Increases or decreases the characters HP
        /// </summary
        public int HP { get; set; }
        /// <summary>
        /// Increases or decreases the characters Mana
        /// </summary
        public int Mana { get; set; }
        /// <summary>
        /// Increases or decreases the characters Moves
        /// </summary
        public int Moves { get; set; }
        /// <summary>
        /// Increases or decreases the characters SpellDam
        /// </summary
        public int SpellDam { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
    }
}
