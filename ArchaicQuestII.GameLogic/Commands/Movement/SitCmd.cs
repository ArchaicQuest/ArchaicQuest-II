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
    public SitCmd()
    {
        Aliases = new[] { "sit" };
        Description =
            "Your character will sit down or sit upon something. Sitting does not increase the speed of health, mana, or moves regeneration."
            + " Only resting or sleeping will do that, if attacked while sitting it will be a critical hit."
            + "<br /><br />Examples<br />sit<br />sit stool";
        Usages = new[] { "Type: sit stool" };
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
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
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
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            SetCharacterStatus(player, "is sitting here", CharacterStatus.Status.Sitting);
            Services.Instance.Writer.WriteLine("<p>You sit down.</p>", player);
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} sits down.</p>",
                room,
                player
            );
        }
        else
        {
            var obj = room.Items.FirstOrDefault(
                x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
            );

            if (obj == null)
            {
                Services.Instance.Writer.WriteLine("<p>You can't sit on that.</p>", player);
                return;
            }

            SetCharacterStatus(
                player,
                $"is sitting down on {obj.Name.ToLower()}",
                CharacterStatus.Status.Sitting
            );
            Services.Instance.Writer.WriteLine(
                $"<p>You sit down on {obj.Name.ToLower()}.</p>",
                player
            );
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} sits down on {obj.Name.ToLower()}.</p>",
                room,
                player
            );
        }
    }

    private void SetCharacterStatus(Player player, string longName, CharacterStatus.Status status)
    {
        player.Status = status;
        player.LongName = longName;
        player.Pose = "";
    }
}
