namespace ArchaicQuestII.GameLogic.Core;

public interface IErrorLog
{
    void Write(string file, string error, ErrorLog.Priority priority);
}
