using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class ConsiderCmd : ICommand
{
    public ConsiderCmd()
    {
        Aliases = new[] { "con", "consider" };
        Description =
            "Consider tells you what your chances are of killing a character. Of course, it's only a rough estimate.";
        Usages = new[] { "Type: consider rat" };
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
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Consider killing who?</p>", player);
            return;
        }

        var victim =
            room.Mobs
                .Where(x => x.IsHiddenScriptMob == false)
                .FirstOrDefault(
                    x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
                )
            ?? room.Players.FirstOrDefault(
                x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)
            );

        if (victim == null)
        {
            Services.Instance.Writer.WriteLine("<p>Consider killing who?</p>", player);
            return;
        }

        if (victim == player)
        {
            Services.Instance.Writer.WriteLine("<p>You could take yourself.</p>", player);
            return;
        }

        if (!victim.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            Services.Instance.Writer.WriteLine("<p>You would need a lot of luck!</p>", player);
            return;
        }

        var diff = victim.Level - player.Level;

        switch (diff)
        {
            case <= -10:
                Services.Instance.Writer.WriteLine("<p>Now where did that chicken go?</p>", player);
                break;
            case <= -5:
                Services.Instance.Writer.WriteLine("<p>You could do it with a needle!</p>", player);
                break;
            case <= -2:
                Services.Instance.Writer.WriteLine("<p>Easy.</p>", player);
                break;
            case <= -1:
                Services.Instance.Writer.WriteLine("<p>Fairly easy.</p>", player);
                break;
            case 0:
                Services.Instance.Writer.WriteLine("<p>The perfect match!</p>", player);
                break;
            case <= 1:
                Services.Instance.Writer.WriteLine("<p>You would need some luck!</p>", player);
                break;
            case <= 2:
                Services.Instance.Writer.WriteLine("<p>You would need a lot of luck!</p>", player);
                break;
            case <= 3:
                Services.Instance.Writer.WriteLine(
                    "<p>You would need a lot of luck and great equipment!</p>",
                    player
                );
                break;
            case <= 5:
                Services.Instance.Writer.WriteLine("<p>Do you feel lucky, punk?</p>", player);
                break;
            case <= 10:
                Services.Instance.Writer.WriteLine("<p>Are you mad!?</p>", player);
                break;
            case <= 100:
                Services.Instance.Writer.WriteLine(
                    "<p>You ARE mad!? Death stands beside you ready to take your soul.</p>",
                    player
                );
                break;
        }
    }
}
