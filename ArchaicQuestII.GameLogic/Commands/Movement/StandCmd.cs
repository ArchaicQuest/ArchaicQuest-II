using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class StandCmd : ICommand
{
    public StandCmd(ICore core)
    {
        Aliases = new[] {"stand"};
        Description = "You character stands up.";
        Usages = new[] {"Type: stand"};
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
            CharacterStatus.Status.Fleeing,
            CharacterStatus.Status.Incapacitated,
            CharacterStatus.Status.Stunned,
        };
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            Core.Writer.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Standing)
        {
            Core.Writer.WriteLine("<p>You are already standing!</p>", player.ConnectionId);
            return;
        }

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
        
        foreach (var pc in room.Players)
        {
            if (pc.Id.Equals(player.Id))
            {
                Core.Writer.WriteLine("<p>You move quickly to your feet.</p>", player.ConnectionId);
            }
            else
            {
                Core.Writer.WriteLine($"<p>{player.Name} {standMessage}</p>", pc.ConnectionId);
            }
        }
    }

    private void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
    {
        player.Status = status;
        player.LongName = longName;
        player.Pose = "";
    }
}