using ArchaicQuestII.DataAccess;

namespace ArchaicQuestII.GameLogic.Core;

public interface IErrorLog
{
    public IDataBase Database { get; }

    void Write(string file, string error, ErrorLog.Priority priority);
}