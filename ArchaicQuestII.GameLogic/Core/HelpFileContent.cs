namespace ArchaicQuestII.GameLogic.Core;

public class HelpFileContent
{
    /// <summary>
    /// Help Title
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Command Aliases
    /// </summary>
    public string[] Aliases { get; set; }
    /// <summary>
    /// Help Description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// How to use command
    /// </summary>
    public string[] Usages { get; set; }

}