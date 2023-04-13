using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class KillCmd : ICommand
{
    public KillCmd()
    {
        Aliases = new[] { "k", "kill", "murder" };
        Description =
            "The kill command is used to begin combat against mobiles.  Once combat has been "
            + "initiated, your character will automatically continue to fight using automatic "
            + "kills which include offensive use of weapons and defenses such as parry, "
            + "shield block and dodge.  Other means of attack may also be used, but will not "
            + " happen automatically. <br /><br /> To kill players you must MURDER them";
        Usages = new[] { "Type: kill rat, murder Arthur" };
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Sleeping,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Resting
        };
        UserRole = UserRole.Player;
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var command = input.ElementAtOrDefault(0);
        var target = input.ElementAtOrDefault(1);
        var isMurder = command == "murder";

        Services.Instance.Combat.Fight(player, target, room, isMurder);
    }
}
