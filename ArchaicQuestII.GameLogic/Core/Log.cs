using System;

namespace ArchaicQuestII.Log
{
    public class Log : ILog
    {
        public void Information(string logInfo)
        {
            //Serilog.Log.Information(logInfo);
        }
        public void Error(string logInfo)
        {
            //Serilog.Log.Error(logInfo);
        }
 
    }
}
