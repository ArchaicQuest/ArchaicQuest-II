using System;
using System.Text.RegularExpressions;

namespace ArchaicQuestII.GameLogic.Utilities
{
    public static class DiceBag
    {
        private const string Modifier = @"\+(.*)";
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

            if (int.TryParse(die, out var num))
            {
                return num;
            }
            
            if (die.StartsWith('+'))
            {
                RollAdvantage(die[1..]);
            }

            if (die.StartsWith('-'))
            {
                RollDisadvantage(die[1..]);
            }
            
            Parse(die, out var numDice, out var dieSize,out var modifier);
            
            var sum = modifier;

            for (var i = 0; i < numDice; i++)
            {
                sum += Throw(1, dieSize + 1);
            }

            return sum;
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

        private static int RollAdvantage(string die)
        {
            Parse(die, out var numDice, out var dieSize,out var modifier);
            
            var die1 = modifier;
            
            for (var i = 0; i < numDice; i++)
            {
                die1 += Throw(1, dieSize + 1);
            }

            var die2 = modifier;
            
            for (var i = 0; i < numDice; i++)
            {
                die2 += Throw(1, dieSize + 1);
            }

            return die1 > die2 ? die1 : die2;

        }

        private static int RollDisadvantage(string die)
        {
            Parse(die, out var numDice, out var dieSize,out var modifier);
            
            var die1 = modifier;
            
            for (var i = 0; i < numDice; i++)
            {
                die1 += Throw(1, dieSize + 1);
            }

            var die2 = modifier;
            
            for (var i = 0; i < numDice; i++)
            {
                die2 += Throw(1, dieSize + 1);
            }

            return die1 < die2 ? die1 : die2;

        }

        /// <summary>
        /// Flip a coin for true or false
        /// </summary>
        /// <returns>True or False</returns>
        public static bool FlipCoin()
        {
            return Throw(0, 2) == 1;
        }

        private static void Parse(string die, out int numDice, out int dieSize, out int modifier)
        {
            die = die.ToLowerInvariant().Trim();
            
            var parsedDie = die.Replace(" ", "");

            if(die.Contains('+'))
                modifier = int.Parse(new Regex(Modifier).ToString());
            else if (die.Contains('-'))
                modifier = int.Parse(new Regex(Modifier).ToString()) * -1;
            else
                modifier = 0;

            var parts = parsedDie.Split("d");
            
            numDice = int.Parse(parts[0]);
            
            dieSize = int.Parse(parts[1]);
        }
    }
}