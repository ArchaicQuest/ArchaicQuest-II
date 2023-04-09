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
        Aliases = new[] {"con", "consider"};
        Description = "Consider tells you what your chances are of killing a character. Of course, it's only a rough estimate.";
        Usages = new[] {"Type: consider rat"};
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
            CoreHandler.Instance.Writer.WriteLine("<p>Consider killing who?</p>", player.ConnectionId);
            return;
        }
        
        var victim =
            room.Mobs.Where(x => x.IsHiddenScriptMob == false).FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
            room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (victim == null)
        {
            CoreHandler.Instance.Writer.WriteLine("<p>Consider killing who?</p>", player.ConnectionId);
            return;
        }

        if (victim == player)
        {
            CoreHandler.Instance.Writer.WriteLine("<p>You could take yourself.</p>", player.ConnectionId);
            return;
        }

        if (!victim.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            CoreHandler.Instance.Writer.WriteLine("<p>You would need a lot of luck!</p>", player.ConnectionId);
            return;
        }

        var diff = victim.Level - player.Level;

        switch (diff)
        {
            case <= -10:
                CoreHandler.Instance.Writer.WriteLine("<p>Now where did that chicken go?</p>", player.ConnectionId);
                break;
            case <= -5:
                CoreHandler.Instance.Writer.WriteLine("<p>You could do it with a needle!</p>", player.ConnectionId);
                break;
            case <= -2:
                CoreHandler.Instance.Writer.WriteLine("<p>Easy.</p>", player.ConnectionId);
                break;
            case <= -1:
                CoreHandler.Instance.Writer.WriteLine("<p>Fairly easy.</p>", player.ConnectionId);
                break;
            case 0:
                CoreHandler.Instance.Writer.WriteLine("<p>The perfect match!</p>", player.ConnectionId);
                break;
            case <= 1:
                CoreHandler.Instance.Writer.WriteLine("<p>You would need some luck!</p>", player.ConnectionId);
                break;
            case <= 2:
                CoreHandler.Instance.Writer.WriteLine("<p>You would need a lot of luck!</p>", player.ConnectionId);
                break;
            case <= 3:
                CoreHandler.Instance.Writer.WriteLine("<p>You would need a lot of luck and great equipment!</p>", player.ConnectionId);
                break;
            case <= 5:
                CoreHandler.Instance.Writer.WriteLine("<p>Do you feel lucky, punk?</p>", player.ConnectionId);
                break;
            case <= 10:
                CoreHandler.Instance.Writer.WriteLine("<p>Are you mad!?</p>", player.ConnectionId);
                break;
            case <= 100:
                CoreHandler.Instance.Writer.WriteLine("<p>You ARE mad!? Death stands beside you ready to take your soul.</p>", player.ConnectionId);
                break;
        }
    }
}