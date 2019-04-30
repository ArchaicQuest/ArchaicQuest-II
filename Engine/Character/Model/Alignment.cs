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
        ///   
        ///Evil    - Alignment between and including -1000 and -350
        ///Neutral - Alignment between and including -349 and 349
        ///Good    - Alignment between and including 350 and 1000
        /// </summary>
        public int Value { get; set; }

   
    }
}
