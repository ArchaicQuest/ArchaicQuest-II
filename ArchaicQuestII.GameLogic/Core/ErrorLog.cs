using ArchaicQuestII.DataAccess;

namespace ArchaicQuestII.GameLogic.Core;

public class ErrorLog : IErrorLog
{
    public IDataBase Database { get; }
    
    public ErrorLog(IDataBase db)
    {
        Database = db;
    }

    /// <summary>
    /// Logs and error to the database
    /// </summary>
    /// <param name="file">which file its in</param>
    /// <param name="error">the error message</param>
    /// <param name="priority"></param>
    public void Write(string file, string error, Priority priority)
    {
        var e = new ErrorInfo
        {
            File = file,
            Error = error,
            Priority = priority
        };
        
        Database.Save(e, DataBase.Collections.ErrorLog);
    }

    public struct ErrorInfo
    {
        public string File;
        public string Error;
        public Priority Priority;
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}