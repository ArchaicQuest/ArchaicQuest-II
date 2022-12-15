using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character;

public class GroupCmd : ICommand
{
    public GroupCmd(ICore core)
    {
        Aliases = new[] {"group"};
        Description = "Forms a group.";
        Usages = new[] {"Type: group bob"};
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }


    public void Execute(Player player, Room room, params string[] input)
    {
        var target = input.ElementAtOrDefault(1);
        
        if ((string.IsNullOrEmpty(target) || target.Equals("group", StringComparison.CurrentCultureIgnoreCase)) && !player.Grouped)
        {
            Core.Writer.WriteLine($"<p>But you are not the member of a group!</p>", player.ConnectionId);
            return;
        }

        if (target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            Core.Writer.WriteLine($"<p>You can't group yourself.</p>", player.ConnectionId);
            return;
        }

        if (target.Equals("list", StringComparison.CurrentCultureIgnoreCase) && player.Grouped)
        {
            Core.Writer.WriteLine($"<p>Grouped with:</p>", player.ConnectionId);

            var sb = new StringBuilder();
            sb.Append("<ul>");

            Player foundLeader;

            if (player.Grouped && player.Followers.Count > 0)
            {
                foundLeader = player;
            }
            else
            {
                foundLeader = Core.Cache.GetPlayerCache()
                    .FirstOrDefault(x => x.Value.Name.StartsWith(player.Following, StringComparison.CurrentCultureIgnoreCase)).Value;
            }

            sb.Append($"<li>Lvl: {foundLeader.Level} {foundLeader.Name} (Leader)  <span class='group-hp' title='Hit points'>{foundLeader.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{foundLeader.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{foundLeader.Attributes.Attribute[EffectLocation.Moves]}</span></li>");

            foreach (var follower in foundLeader.Followers.Where(x => x.Grouped))
            {
                sb.Append($"<li>Lvl: {follower.Level} {follower.Name} <span class='group-hp' title='Hit points'>{follower.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{follower.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{follower.Attributes.Attribute[EffectLocation.Moves]}</span></li>");
            }

            sb.Append("</ul>");
            Core.Writer.WriteLine(sb.ToString(), player.ConnectionId);

            return;
        }

        var foundPlayer = player.Followers
            .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (foundPlayer == null)
        {
            Core.Writer.WriteLine("<p>They are not following you!</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer == player)
        {
            Core.Writer.WriteLine("<p>You can't group with yourself!</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer.Grouped)
        {
            foundPlayer.Grouped = false;

            Core.Writer.WriteLine($"<p>{foundPlayer.Name} is no longer a member of your group.</p>", player.ConnectionId);
            Core.Writer.WriteLine($"<p>You are no longer a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);
            return;

        }

        foundPlayer.Grouped = true;
        player.Grouped = true;
        Core.Writer.WriteLine($"<p>{foundPlayer.Name} is now a member of your group.</p>", player.ConnectionId);
        Core.Writer.WriteLine($"<p>You are now a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);

        foreach (var pc in room.Players.Where(pc => pc.Id != player.Id && pc.Id != foundPlayer.Id))
        {
            Core.Writer.WriteLine($"<p>{foundPlayer.Name} is now a member of {player.Name}'s group.</p>", pc.ConnectionId);
        }
    }
}