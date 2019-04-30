using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Core.Interface;

namespace ArchaicQuestII.Engine.Character.Model
{
    public class Alignment: Option
    {
        /// <summary>
        /// Value determines Alignment value
        /// if player is -1000 value they will be lawful good
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets an alignment string from an alignment value.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static string AlignmentString(int alignment)
        {
            if (alignment > 1000)
                return "buggy - too high";
            if (alignment == 1000)
                return "pure and holy";
            if (alignment > 900)
                return "extremely good";
            if (alignment > 600)
                return "very good";
            if (alignment > 350)
                return "good";
            if (alignment > 100)
                return "neutral leaning towards good";
            if (alignment > -100)
                return "neutral";
            if (alignment > -350)
                return "neutral leaning towards evil";
            if (alignment > -600)
                return "evil";
            if (alignment > -900)
                return "very evil";
            if (alignment > -1000)
                return "extremely evil";
            if (alignment == -1000)
                return "pure evil";
            return "buggy - too low";
        }
    }
}
