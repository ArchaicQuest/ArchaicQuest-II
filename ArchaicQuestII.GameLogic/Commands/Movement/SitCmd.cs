using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class SitCmd : ICommand
{
    public SitCmd(ICore core)
    {
        Aliases = new[] {"sit"};
        Description = "Sits on something.";
        Usages = new[] {"Type: sit stool"};
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
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
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
        var target = input.ElementAtOrDefault(1);

        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            Core.Writer.WriteLine("<p>You can't do that while mounted.</p>", player.ConnectionId);
            return;
        }

        if (player.Status == CharacterStatus.Status.Sitting)
        {
            Core.Writer.WriteLine("<p>You are already sitting!</p>", player.ConnectionId);
            return;
        }

        if (string.IsNullOrEmpty(target))
        {
            SetCharacterStatus(player, "is sitting here", CharacterStatus.Status.Sitting);
            Core.Writer.WriteLine("<p>You sit down.</p>", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"<p>{player.Name} sits down.</p>", room, player);
            
        }
        else
        {
            var obj = room.Items.FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            if (obj == null)
            {
                Core.Writer.WriteLine("<p>You can't sit on that.</p>", player.ConnectionId);
                return;
            }

            SetCharacterStatus(player, $"is sitting down on {obj.Name.ToLower()}", CharacterStatus.Status.Sitting);
            Core.Writer.WriteLine($"<p>You sit down on {obj.Name.ToLower()}.</p>", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"<p>{player.Name} sits down on {obj.Name.ToLower()}.</p>", room, player);
        }
    }

    private void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
    {
        player.Status = status;
        player.LongName = longName;
        player.Pose = "";
    }
}