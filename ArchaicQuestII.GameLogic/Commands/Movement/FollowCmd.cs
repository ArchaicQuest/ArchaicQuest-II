using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class FollowCmd : ICommand
{
    public FollowCmd(ICore core)
    {
        Aliases = new[] {"follow", "fol"};
        Description = "Follows another character.";
        Usages = new[] {"Type: follow liam"};
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
            Core.Writer.WriteLine($"Follow who?", player.ConnectionId);
            return;
        }
        
        if (target.Equals("self", StringComparison.CurrentCultureIgnoreCase) || target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            var leader = Core.Cache.GetPlayerCache()
          .FirstOrDefault(x => x.Value.Name.Equals(string.IsNullOrEmpty(player.Following) ? player.Name : player.Following, StringComparison.CurrentCultureIgnoreCase));

            Core.Writer.WriteLine($"<p>You stop following {leader.Value.Name}.</p>", player.ConnectionId);
            if (player.Name != leader.Value.Name)
            {
                Core.Writer.WriteLine($"<p>{player.Name} stops following you.</p>", leader.Value.ConnectionId);
            }

            leader.Value.Followers.Remove(player);
            if (leader.Value.Followers.Count == 0)
            {
                leader.Value.Grouped = false;
            }
            player.Following = null;
            player.Grouped = false;


            return;
        }

        var foundPlayer = room.Players
            .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (foundPlayer == null)
        {
            Core.Writer.WriteLine("<p>You don't see them here.</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer.Followers.Contains(player))
        {
            Core.Writer.WriteLine($"<p>You are already following {foundPlayer.Name}.</p>", player.ConnectionId);
            return;
        }
        
        if (foundPlayer.Following == player.Name)
        {
            Core.Writer.WriteLine($"<p>You can't follow someone following you. Lest you be running around in circles indefinitely.</p>", player.ConnectionId);
            return;
        }
        
        if (foundPlayer.Config.CanFollow == false)
        {
            Core.Writer.WriteLine($"<p>{foundPlayer.Name} doesn't want to be followed.</p>", player.ConnectionId);
            return;
        }

        Core.Writer.WriteLine($"<p>{player.Name} now follows you.</p>", foundPlayer.ConnectionId);
        Core.Writer.WriteLine($"<p>You are now following {foundPlayer.Name}.</p>", player.ConnectionId);

        player.Following = foundPlayer.Name;
        foundPlayer.Followers.Add(player);
    }
}