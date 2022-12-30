using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class CastCmd : ICommand
{
    public CastCmd(ICore core)
    {
        Aliases = new[] {"cast", "c"};
        Description = "Before you can cast a spell, you have to practice it.  The more you practice, " +
                      "the higher chance you have of success when casting.  Casting spells costs mana. " +
                      "The mana cost decreases as your level increases. " +
                      "The target is optional.  Many spells which need targets will use an " +
                      " appropriate default target, especially during combat.";
        Usages = new[] {"Type: cast fireball, cast <spell> <target>"};
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Floating,
            CharacterStatus.Status.Mounted,
            CharacterStatus.Status.Sitting,
            CharacterStatus.Status.Resting
        };
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }


    public void Execute(Player player, Room room, string[] input)
    {
        //TODO: Build out spells like new commands
    }
}