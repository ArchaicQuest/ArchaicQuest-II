namespace ArchaicQuestII.GameLogic.Core;

public struct ErrorInfo
{
    public string File;
    public string Error;
    public ErrorPriority Priority;
}

public enum ErrorPriority
{
    Low,
    Medium,
    High
}