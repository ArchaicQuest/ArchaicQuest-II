using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Help
{
    public interface IHelp
    {
        List<Help> FindHelpFile(string keyword);
        void DisplayHelpFile(string keyword, Player player);
    }
}
