using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//copy pasta https://stackoverflow.com/a/20595549/1395510
namespace ArchaicQuestII.GameLogic.Core
{
    public interface IWriteToClient
    {

        void WriteLineMobSay(string mobName, string message, string id);
        void WriteLine(string message, string id);
        void WriteLine(string message, string id, int delay);
        void WriteLine(string message);
        void WriteLineRoom(string message, string id, int delay);
    }
}
