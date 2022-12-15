using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Combat;

public class ConsiderCmd : ICommand
{
    public ConsiderCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"consider"};
        Description = "See your chances of defeating an enemy.";
        Usages = new[] {"Type: consider rat"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Writer.WriteLine("Consider killing who?", player.ConnectionId);
            return;
        }
        
        var victim =
            room.Mobs.Where(x => x.IsHiddenScriptMob == false).FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)) ??
            room.Players.FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (victim == null)
        {
            Writer.WriteLine("Consider killing who?", player.ConnectionId);
            return;
        }

        if (victim == player)
        {
            Writer.WriteLine("You could take yourself.", player.ConnectionId);
            return;
        }

        if (!victim.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
        {
            Writer.WriteLine("You would need a lot of luck!", player.ConnectionId);
            return;
        }

        var diff = victim.Level - player.Level;

        switch (diff)
        {
            case <= -10:
                Writer.WriteLine("Now where did that chicken go?", player.ConnectionId);
                break;
            case <= -5:
                Writer.WriteLine("You could do it with a needle!", player.ConnectionId);
                break;
            case <= -2:
                Writer.WriteLine("Easy.", player.ConnectionId);
                break;
            case <= -1:
                Writer.WriteLine("Fairly easy.", player.ConnectionId);
                break;
            case 0:
                Writer.WriteLine("The perfect match!", player.ConnectionId);
                break;
            case <= 1:
                Writer.WriteLine("You would need some luck!", player.ConnectionId);
                break;
            case <= 2:
                Writer.WriteLine("You would need a lot of luck!", player.ConnectionId);
                break;
            case <= 3:
                Writer.WriteLine("You would need a lot of luck and great equipment!", player.ConnectionId);
                break;
            case <= 5:
                Writer.WriteLine("Do you feel lucky, punk?", player.ConnectionId);
                break;
            case <= 10:
                Writer.WriteLine("Are you mad!?", player.ConnectionId);
                break;
            case <= 100:
                Writer.WriteLine("You ARE mad!? Death stands beside you ready to take your soul.", player.ConnectionId);
                break;
        }
    }
}