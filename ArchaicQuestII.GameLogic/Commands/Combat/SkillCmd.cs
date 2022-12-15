using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class SkillCmd : ICommand
{
    public SkillCmd(ICore core)
    {
        Aliases = new[] {"skill"};
        Description = "Use one of your skills.";
        Usages = new[] {"Type: skill cleave"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }


    public void Execute(Player player, Room room, string[] input)
    {
        //TODO: Build out skills like new commands
    }
}