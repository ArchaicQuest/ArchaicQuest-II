using ArchaicQuestII.DataAccess;

namespace ArchaicQuestII.GameLogic.Core;

public interface IErrorLog
{
    public DataBase Database { get; }

    void Write(string file, string error, ErrorLog.Priority priority);
}