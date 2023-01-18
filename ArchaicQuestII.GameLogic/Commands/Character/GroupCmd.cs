using System;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Character;

public class GroupCmd : ICommand
{
    public GroupCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"group"};
        Description = "Playing with others is more fun, to earn XP together you must be in a group. <br /> <br />To start a group, request" +
                      " the person you wish to join to follow you. Once followed enter group (player name) you now have a group.<br /> <br /> Follow the same steps to add more players." +
                      "<br /> <br />To leave or disband a group enter follow self, to view the group enter group list.<br /> <br />To communicate with the group use gsay hello pr groupsay hello to talk privately in the group.<br /> <br />" +
                      "Syntax:<br />" +
                      "group (player name) - group liam, for liam to join your group. Liam must be following you <br />" +
                      "group list - view who's in the group<br />" +
                      "gsay (text) - gsay let's go on an adventure.";
        Usages = new[] {"Type: group bob, group list, gsay hello"};
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
            CharacterStatus.Status.Stunned
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


    public void Execute(Player player, Room room, params string[] input)
    {
        var target = input.ElementAtOrDefault(1);
        
        if ((string.IsNullOrEmpty(target) || target.Equals("group", StringComparison.CurrentCultureIgnoreCase)) && !player.Grouped)
        {
            Handler.Client.WriteLine("<p>But you are not the member of a group!</p>", player.ConnectionId);
            return;
        }

        if (target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            Handler.Client.WriteLine($"<p>You can't group yourself.</p>", player.ConnectionId);
            return;
        }

        if (target.Equals("list", StringComparison.CurrentCultureIgnoreCase) && player.Grouped)
        {
            Handler.Client.WriteLine("<p>Grouped with:</p>", player.ConnectionId);

            var sb = new StringBuilder();
            sb.Append("<ul>");

            Player foundLeader;

            if (player.Grouped && player.Followers.Count > 0)
            {
                foundLeader = player;
            }
            else
            {
                foundLeader = Handler.Character.GetPlayerCache()
                    .FirstOrDefault(x => x.Value.Name.StartsWith(player.Following, StringComparison.CurrentCultureIgnoreCase)).Value;
            }

            sb.Append($"<li>Lvl: {foundLeader.Level} {foundLeader.Name} (Leader)  <span class='group-hp' title='Hit points'>{foundLeader.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{foundLeader.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{foundLeader.Attributes.Attribute[EffectLocation.Moves]}</span></li>");

            foreach (var follower in foundLeader.Followers.Where(x => x.Grouped))
            {
                sb.Append($"<li>Lvl: {follower.Level} {follower.Name} <span class='group-hp' title='Hit points'>{follower.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{follower.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{follower.Attributes.Attribute[EffectLocation.Moves]}</span></li>");
            }

            sb.Append("</ul>");
            Handler.Client.WriteLine(sb.ToString(), player.ConnectionId);

            return;
        }

        var foundPlayer = player.Followers
            .FirstOrDefault(x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));

        if (foundPlayer == null)
        {
            Handler.Client.WriteLine("<p>They are not following you!</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer == player)
        {
            Handler.Client.WriteLine("<p>You can't group with yourself!</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer.Grouped)
        {
            foundPlayer.Grouped = false;

            Handler.Client.WriteLine($"<p>{foundPlayer.Name} is no longer a member of your group.</p>", player.ConnectionId);
            Handler.Client.WriteLine($"<p>You are no longer a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);
            return;

        }

        foundPlayer.Grouped = true;
        player.Grouped = true;
        Handler.Client.WriteLine($"<p>{foundPlayer.Name} is now a member of your group.</p>", player.ConnectionId);
        Handler.Client.WriteLine($"<p>You are now a member of {player.Name}'s group.</p>", foundPlayer.ConnectionId);

        foreach (var pc in room.Players.Where(pc => pc.Id != player.Id && pc.Id != foundPlayer.Id))
        {
            Handler.Client.WriteLine($"<p>{foundPlayer.Name} is now a member of {player.Name}'s group.</p>", pc.ConnectionId);
        }
    }
}