using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class SleepCmd : ICommand
{
    public SleepCmd()
    {
        Aliases = new[] { "sleep" };
        Description =
            "Your character will go to sleep and will not see anything that happens the room. Sleeping will increase the speed of health, mana, and moves regeneration."
            + " Make sure you are somewhere safe because if attacked it will be a guaranteed critical hit.<br /><br />To wake up enter stand or wake.";
        Usages = new[] { "Type: sleep" };
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
            CharacterStatus.Status.Mounted
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
        SetCharacterStatus(player, "is sleeping nearby", CharacterStatus.Status.Sleeping);
        Services.Instance.Writer.WriteLine("<p>You collapse into a deep sleep.</p>", player);
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} collapses into a deep sleep.</p>",
            room,
            player
        );
    }

    private void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
    {
        player.Status = status;
        player.LongName = longName;
        player.Pose = "";
    }
}
