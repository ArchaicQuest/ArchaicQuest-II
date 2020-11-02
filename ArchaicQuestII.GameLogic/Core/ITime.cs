using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ITime
    {
        public void UpdateTime();
        public string ReturnTime();
        public bool IsNightTime();
    }
}
