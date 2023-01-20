using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Alignment
{
    public class Alignment : Option
    {
        /// <summary>
        /// Value determines Alignment value
        ///   
        ///Evil    - Alignment between and including -1000 and -350
        ///Neutral - Alignment between and including -349 and 349
        ///Good    - Alignment between and including 350 and 1000
        /// </summary>
        public int Value { get; set; }
    }
}
