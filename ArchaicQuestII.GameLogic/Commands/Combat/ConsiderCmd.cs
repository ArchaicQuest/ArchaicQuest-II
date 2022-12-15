using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class ConsiderCmd : ICommand
{
    public ConsiderCmd(ICore core)
    {
        Aliases = new[] {"con", "consider"};
        Description = "See your chances of defeating an enemy.";
        Usages = new[] {"Type: consider rat"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("Consider killing who?", player.ConnectionId);
            return;
        }
        
        var victim =
            room.Mobs.Where(x => x.IsHiddenScriptMob == false).FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
            room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (victim == null)
        {
            Core.Writer.WriteLine("Consider killing who?", player.ConnectionId);
            return;
        }

        if (victim == player)
        {
            Core.Writer.WriteLine("You could take yourself.", player.ConnectionId);
            return;
        }

        if (!victim.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            Core.Writer.WriteLine("You would need a lot of luck!", player.ConnectionId);
            return;
        }

        var diff = victim.Level - player.Level;

        switch (diff)
        {
            case <= -10:
                Core.Writer.WriteLine("Now where did that chicken go?", player.ConnectionId);
                break;
            case <= -5:
                Core.Writer.WriteLine("You could do it with a needle!", player.ConnectionId);
                break;
            case <= -2:
                Core.Writer.WriteLine("Easy.", player.ConnectionId);
                break;
            case <= -1:
                Core.Writer.WriteLine("Fairly easy.", player.ConnectionId);
                break;
            case 0:
                Core.Writer.WriteLine("The perfect match!", player.ConnectionId);
                break;
            case <= 1:
                Core.Writer.WriteLine("You would need some luck!", player.ConnectionId);
                break;
            case <= 2:
                Core.Writer.WriteLine("You would need a lot of luck!", player.ConnectionId);
                break;
            case <= 3:
                Core.Writer.WriteLine("You would need a lot of luck and great equipment!", player.ConnectionId);
                break;
            case <= 5:
                Core.Writer.WriteLine("Do you feel lucky, punk?", player.ConnectionId);
                break;
            case <= 10:
                Core.Writer.WriteLine("Are you mad!?", player.ConnectionId);
                break;
            case <= 100:
                Core.Writer.WriteLine("You ARE mad!? Death stands beside you ready to take your soul.", player.ConnectionId);
                break;
        }
    }
}