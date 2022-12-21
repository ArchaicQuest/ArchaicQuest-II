using System;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Dice : IDice
    {
        private static readonly Random Throws = new((int)DateTime.Now.Ticks);

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
        public int DiceMinSize { get; set; }
        public int DiceMaxSize { get; set; }

        public int Roll(int roll, int minSize, int maxSize)
        {
            var total = 0;

            for (var i = 0; i < roll; i++)
            {
                total += Throws.Next(minSize, maxSize + 1);
            }

            return total;
        }
    }
}
