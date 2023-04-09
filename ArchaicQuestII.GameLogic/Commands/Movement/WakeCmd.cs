using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class WakeCmd : ICommand
{
    public WakeCmd()
    {
        Aliases = new[] { "wake" };
        Description = "Your character wakes from sleep.";
        Usages = new[] { "Type: wake" };
        Title = "";
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Stunned,
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
            CharacterStatus.Status.Standing,
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
        SetCharacterStatus(player, "", CharacterStatus.Status.Standing);
        CoreHandler.Instance.Writer.WriteLine(
            "<p>You move quickly to your feet.</p>",
            player.ConnectionId
        );
        CoreHandler.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} arises from {(player.Gender == "Male" ? "his" : "her")} slumber.</p>",
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
