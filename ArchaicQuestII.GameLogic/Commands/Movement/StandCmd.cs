using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class StandCmd : ICommand
{
    public StandCmd()
    {
        Aliases = new[] { "stand" };
        Description = "You character stands up.";
        Usages = new[] { "Type: stand" };
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
            CharacterStatus.Status.Mounted,
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
        var standMessage = "rises up.";

        if (player.Status == CharacterStatus.Status.Resting)
        {
            standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} rest.";
        }
        else if (player.Status == CharacterStatus.Status.Sleeping)
        {
            standMessage = $"arises from {(player.Gender == "Male" ? "his" : "her")} slumber.";
        }

        SetCharacterStatus(player, "", CharacterStatus.Status.Standing);
        Services.Instance.Writer.WriteLine(
            "<p>You move quickly to your feet.</p>",
            player.ConnectionId
        );
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} {standMessage}</p>",
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
