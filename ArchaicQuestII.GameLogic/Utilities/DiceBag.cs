using System;

namespace ArchaicQuestII.GameLogic.Utilities
{
    public static class DiceBag
    {
        private static readonly Random Random = new(); 
        private static readonly object SyncLock = new();

        private static int Throw(int min, int max)
        {
            lock(SyncLock)
                return Random.Next(min, max);
        }

        /// <summary>
        /// Rolls a die based on string;
        /// example of one, six sided die: 1d6
        /// example of three, four sided die: 3d4
        /// example of one, six sided die with advantage: +1d6
        /// example of one, six sided die with disadvantage: -1d6
        /// example of one, six sided die with positive two modifier: 1d6+2
        /// example of one, six sided die with negative two modifier: 1d6-2
        /// </summary>
        /// <param name="die"></param>
        /// <returns></returns>
        public static int Roll(string die)
        {
            if (string.IsNullOrEmpty(die)) return 0;

            if (!TryParse(die, out var numDice, out var dieSize, out var modifier, out var advantage))
                return modifier;

            if (advantage > 0)
                return RollAdvantage(numDice, dieSize, modifier);
            if (advantage < 0)
                return RollDisadvantage(numDice, dieSize, modifier);
            
            return Roll(numDice, 1, dieSize, modifier);
        }
        
        
        /// <summary>
        /// Rolls a die based on int parameters
        /// </summary>
        /// <param name="numDice">Number if die</param>
        /// <param name="minDieSize">Lowest die number</param>
        /// <param name="maxDieSize">Hightest die number</param>
        /// <param name="modifier">Amount to add/subtract from roll</param>
        /// <returns></returns>
        public static int Roll(int numDice, int minDieSize, int maxDieSize, int modifier = 0)
        {
            var sum = modifier;

            for (var i = 0; i < numDice; i++)
            {
                sum += Throw(minDieSize, maxDieSize + 1);
            }

            return sum;
        }

        private static int RollAdvantage(int numDice, int dieSize, int modifier = 0)
        {
            var die1 = Roll(numDice, 1, dieSize, modifier);
            var die2 = Roll(numDice, 1, dieSize, modifier);
            
            return die1 > die2 ? die1 : die2;
        }

        private static int RollDisadvantage(int numDice, int dieSize, int modifier = 0)
        {
            var die1 = Roll(numDice, 1, dieSize, modifier);
            var die2 = Roll(numDice, 1, dieSize, modifier);

            return die1 < die2 ? die1 : die2;
        }

        public static int[] RollBag(int numDice, int dieSize)
        {
            var diebag = new int[numDice];
            
            for (var i = 0; i < numDice; i++)
            {
                diebag[i] = Roll(1, 1, dieSize);
            }

            return diebag;
        }

        /// <summary>
        /// Flip a coin for true or false
        /// </summary>
        /// <returns>True or False</returns>
        public static bool FlipCoin()
        {
            return Throw(0, 2) == 1;
        }

        private static bool TryParse(string die, out int numDice, out int dieSize, out int modifier, out int advantage)
        {
            numDice = 0;
            dieSize = 0;
            modifier = 0;
            advantage = 0;
            
            die = die.ToLowerInvariant().Trim();
            
            if (int.TryParse(die, out var num))
            {
                modifier = num;
                return false;
            }
            
            var parsedDie = die.Replace(" ", "");
            
            if (die.StartsWith('+'))
            {
                advantage = 1;
                parsedDie = die[1..];
            }

            if (die.StartsWith('-'))
            {
                advantage = -1;
                parsedDie = die[1..];
            }

            var pos = parsedDie.Split('+');
            var neg = parsedDie.Split('-');

            if (pos.Length > 1)
            {
                int.TryParse(pos[1], out modifier);
                parsedDie = pos[0];
            }
            else if (neg.Length > 1)
            {
                int.TryParse(neg[1], out modifier);
                parsedDie = neg[0];
                modifier *= -1;
            }

            var parts = parsedDie.Split("d");

            if (parts.Length < 2)
                return false;

            if (!int.TryParse(parts[0], out numDice))
                return false;
            
            if (!int.TryParse(parts[1], out dieSize))
                return false;

            return true;
        }
    }
}