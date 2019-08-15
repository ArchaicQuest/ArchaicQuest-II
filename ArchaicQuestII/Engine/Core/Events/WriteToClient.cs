using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Core.Events;

namespace ArchaicQuestII.Engine.Core.Events
{
    public class WriteToClient: IWriteToClient
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
