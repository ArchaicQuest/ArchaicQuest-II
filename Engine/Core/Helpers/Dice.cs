using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Item
{
    public class Dice
    {
        /// <summary>
        /// How many rolls for the Dice
        /// e.g 4d6 
        /// will roll four six sided dice
        /// min dam would be 4
        /// max dam would be 24
        /// </summary>
        public int DiceRoll { get; set; }
        /// <summary>
        /// d4, d6, d8, d10, d20
        /// </summary>
        public int DiceSize { get; set; }
    }
}
