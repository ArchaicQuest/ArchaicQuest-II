using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class GsayCmd : ICommand
{
    public GsayCmd(ICore core)
    {
        Aliases = new[] {"gsay"};
        Description = "Sends a message to your current group";
        Usages = new[] {"Type: gsay hello group"};
        DeniedStatus = default;
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
        if (!player.Grouped)
        {
            Core.Writer.WriteLine("You are not in a group.", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("Gsay what?", player.ConnectionId);
            return;
        }
            
        Player foundLeader;

        if (player.Grouped && player.Followers.Count > 0)
        {
            foundLeader = player;
        }
        else
        {
            foundLeader = Core.Cache.GetPlayerCache()
                .FirstOrDefault(x => x.Value.Name.Equals(player.Following, StringComparison.CurrentCultureIgnoreCase)).Value;
        }
        
        var text = string.Join(" ", input.Skip(1));
            
        Core.Writer.WriteLine($"<p class='gsay'>[group] You: <span>{text}</span></p>", player.ConnectionId);

        if (!string.IsNullOrEmpty(player.Following) && foundLeader.Name == player.Following)
        {
            Core.Writer.WriteLine($"<p class='gsay'>[group] {player.Name}: <span>{text}</span></p>", foundLeader.ConnectionId);
        }
            
        foreach (var follower in foundLeader.Followers.Where(follower => !follower.Id.Equals(player.Id)))
        {
            Core.Writer.WriteLine($"<p class='gsay'>[group] {player.Name}: <span>{text}</span></p>", follower.ConnectionId);
        }
    }
}