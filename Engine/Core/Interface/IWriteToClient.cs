using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//copy pasta https://stackoverflow.com/a/20595549/1395510
namespace ArchaicQuestII.Engine.Core.Interface
{
    interface IWriteToClient
    {
            void WriteLine(string message);
    }
}
