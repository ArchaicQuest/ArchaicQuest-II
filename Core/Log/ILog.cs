using System;

namespace ArchaicQuestII.Log
{
    public interface ILog
    {
         void Information (string logInfo);
         void Error (string logInfo);
    }
}
