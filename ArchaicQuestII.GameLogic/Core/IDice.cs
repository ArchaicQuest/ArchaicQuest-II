using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface IDice
    {
        public int Roll(int roll, int minSize, int maxSize);
    }
}
