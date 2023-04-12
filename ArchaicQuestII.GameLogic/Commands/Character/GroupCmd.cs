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
    public GroupCmd()
    {
        Aliases = new[] { "group" };
        Description =
            "Playing with others is more fun, to earn XP together you must be in a group. <br /> <br />To start a group, request"
            + " the person you wish to join to follow you. Once followed enter group (player name) you now have a group.<br /> <br /> Follow the same steps to add more players."
            + "<br /> <br />To leave or disband a group enter follow self, to view the group enter group list.<br /> <br />To communicate with the group use gsay hello pr groupsay hello to talk privately in the group.<br /> <br />"
            + "Syntax:<br />"
            + "group (player name) - group liam, for liam to join your group. Liam must be following you <br />"
            + "group list - view who's in the group<br />"
            + "gsay (text) - gsay let's go on an adventure.";
        Usages = new[] { "Type: group bob, group list, gsay hello" };
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
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, params string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (
            (
                string.IsNullOrEmpty(target)
                || target.Equals("group", StringComparison.CurrentCultureIgnoreCase)
            ) && !player.Grouped
        )
        {
            Services.Instance.Writer.WriteLine(
                "<p>But you are not the member of a group!</p>",
                player
            );
            return;
        }

        if (target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            Services.Instance.Writer.WriteLine($"<p>You can't group yourself.</p>", player);
            return;
        }

        if (target.Equals("list", StringComparison.CurrentCultureIgnoreCase) && player.Grouped)
        {
            Services.Instance.Writer.WriteLine("<p>Grouped with:</p>", player);

            var sb = new StringBuilder();
            sb.Append("<ul>");

            Player foundLeader;

            if (player.Grouped && player.Followers.Count > 0)
            {
                foundLeader = player;
            }
            else
            {
                foundLeader = Services.Instance.Cache
                    .GetPlayerCache()
                    .FirstOrDefault(
                        x =>
                            x.Value.Name.StartsWith(
                                player.Following,
                                StringComparison.CurrentCultureIgnoreCase
                            )
                    )
                    .Value;
            }

            sb.Append(
                $"<li>Lvl: {foundLeader.Level} {foundLeader.Name} (Leader)  <span class='group-hp' title='Hit points'>{foundLeader.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{foundLeader.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{foundLeader.Attributes.Attribute[EffectLocation.Moves]}</span></li>"
            );

            foreach (var follower in foundLeader.Followers.Where(x => x.Grouped))
            {
                sb.Append(
                    $"<li>Lvl: {follower.Level} {follower.Name} <span class='group-hp' title='Hit points'>{follower.Attributes.Attribute[EffectLocation.Hitpoints]}</span>/<span class='group-mana' title='Mana points'>{follower.Attributes.Attribute[EffectLocation.Mana]}</span>/<span class='group-moves' title='Move points'>{follower.Attributes.Attribute[EffectLocation.Moves]}</span></li>"
                );
            }

            sb.Append("</ul>");
            Services.Instance.Writer.WriteLine(sb.ToString(), player);

            return;
        }

        var foundPlayer = player.Followers.FirstOrDefault(
            x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)
        );

        if (foundPlayer == null)
        {
            Services.Instance.Writer.WriteLine("<p>They are not following you!</p>", player);
            return;
        }

        if (foundPlayer == player)
        {
            Services.Instance.Writer.WriteLine("<p>You can't group with yourself!</p>", player);
            return;
        }

        if (foundPlayer.Grouped)
        {
            foundPlayer.Grouped = false;

            Services.Instance.Writer.WriteLine(
                $"<p>{foundPlayer.Name} is no longer a member of your group.</p>",
                player
            );
            Services.Instance.Writer.WriteLine(
                $"<p>You are no longer a member of {player.Name}'s group.</p>",
                foundPlayer
            );
            return;
        }

        foundPlayer.Grouped = true;
        player.Grouped = true;
        Services.Instance.Writer.WriteLine(
            $"<p>{foundPlayer.Name} is now a member of your group.</p>",
            player
        );
        Services.Instance.Writer.WriteLine(
            $"<p>You are now a member of {player.Name}'s group.</p>",
            foundPlayer
        );

        foreach (var pc in room.Players.Where(pc => pc.Id != player.Id && pc.Id != foundPlayer.Id))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{foundPlayer.Name} is now a member of {player.Name}'s group.</p>",
                pc
            );
        }
    }
}
