using System;
namespace ArchaicQuestII.GameLogic.Core
{
    public class WriteToClient: IWriteToClient
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
