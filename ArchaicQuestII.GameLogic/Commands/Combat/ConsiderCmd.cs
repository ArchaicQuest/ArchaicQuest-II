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
    public ConsiderCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"con", "consider"};
        Description = "Consider tells you what your chances are of killing a character. Of course, it's only a rough estimate.";
        Usages = new[] {"Type: consider rat"};
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

        Handler = coreHandler;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Handler.Client.WriteLine("<p>Consider killing who?</p>", player.ConnectionId);
            return;
        }
        
        var victim =
            room.Mobs.Where(x => x.IsHiddenScriptMob == false).FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
            room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (victim == null)
        {
            Handler.Client.WriteLine("<p>Consider killing who?</p>", player.ConnectionId);
            return;
        }

        if (victim == player)
        {
            Handler.Client.WriteLine("<p>You could take yourself.</p>", player.ConnectionId);
            return;
        }

        if (!victim.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            Handler.Client.WriteLine("<p>You would need a lot of luck!</p>", player.ConnectionId);
            return;
        }

        var diff = victim.Level - player.Level;

        var message = diff switch
        {
            <= -10 => "<p>Now where did that chicken go?</p>",
            <= -5 => "<p>You could do it with a needle!</p>",
            <= -2 => "<p>Easy.</p>",
            <= -1 => "<p>Fairly easy.</p>",
            0 => "<p>The perfect match!</p>",
            <= 1 => "<p>You would need some luck!</p>",
            <= 2 => "<p>You would need a lot of luck!</p>",
            <= 3 => "<p>You would need a lot of luck and great equipment!</p>",
            <= 5 => "<p>Do you feel lucky, punk?</p>",
            <= 10 => "<p>Are you mad!?</p>",
            <= 100 => "<p>You ARE mad!? Death stands beside you ready to take your soul.</p>",
            _ => ""
        };

        Handler.Client.WriteLine(message, player.ConnectionId);
    }
}