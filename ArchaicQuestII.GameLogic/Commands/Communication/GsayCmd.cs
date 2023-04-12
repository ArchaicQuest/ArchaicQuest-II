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
    public GsayCmd()
    {
        Aliases = new[] { "gsay", "`", "gs" };
        Description = "Sends a message to your current group";
        Usages = new[] { "Type: gsay hello group" };
        Title = "";
        DeniedStatus = null;
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
        if (!player.Grouped)
        {
            Services.Instance.Writer.WriteLine("<p>You are not in a group.</p>", player);
            return;
        }

        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Services.Instance.Writer.WriteLine("<p>Gsay what?</p>", player);
            return;
        }

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
                        x.Value.Name.Equals(
                            player.Following,
                            StringComparison.CurrentCultureIgnoreCase
                        )
                )
                .Value;
        }

        var text = string.Join(" ", input.Skip(1));

        Services.Instance.Writer.WriteLine(
            $"<p class='gsay'>[group] You: <span>{text}</span></p>",
            player
        );

        if (!string.IsNullOrEmpty(player.Following) && foundLeader.Name == player.Following)
        {
            Services.Instance.Writer.WriteLine(
                $"<p class='gsay'>[group] {player.Name}: <span>{text}</span></p>",
                foundLeader
            );
        }

        foreach (
            var follower in foundLeader.Followers.Where(follower => !follower.Id.Equals(player.Id))
        )
        {
            Services.Instance.Writer.WriteLine(
                $"<p class='gsay'>[group] {player.Name}: <span>{text}</span></p>",
                follower
            );
        }
    }
}
