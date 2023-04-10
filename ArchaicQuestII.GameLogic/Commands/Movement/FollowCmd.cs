using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class FollowCmd : ICommand
{
    public FollowCmd()
    {
        Aliases = new[] { "follow", "fol" };
        Description =
            "'{yellow}follow{/}' starts you following another character. To stop following anyone else, just follow yourself. "
            + "If you don't want to be followed you can turn off 'can follow' from the settings modal.";
        Usages = new[] { "Type: follow liam, follow self" };
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
            Services.Instance.Writer.WriteLine("<p>Follow who?</p>", player.ConnectionId);
            return;
        }

        if (
            target.Equals("self", StringComparison.CurrentCultureIgnoreCase)
            || target.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase)
        )
        {
            var leader = Services.Instance.Cache
                .GetPlayerCache()
                .FirstOrDefault(
                    x =>
                        x.Value.Name.Equals(
                            string.IsNullOrEmpty(player.Following) ? player.Name : player.Following,
                            StringComparison.CurrentCultureIgnoreCase
                        )
                );

            Services.Instance.Writer.WriteLine(
                $"<p>You stop following {leader.Value.Name}.</p>",
                player.ConnectionId
            );
            if (player.Name != leader.Value.Name)
            {
                Services.Instance.Writer.WriteLine(
                    $"<p>{player.Name} stops following you.</p>",
                    leader.Value.ConnectionId
                );
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

        var foundPlayer = room.Players.FirstOrDefault(
            x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)
        );

        if (foundPlayer == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't see them here.</p>",
                player.ConnectionId
            );
            return;
        }

        if (foundPlayer.Followers.Contains(player))
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are already following {foundPlayer.Name}.</p>",
                player.ConnectionId
            );
            return;
        }

        if (foundPlayer.Following == player.Name)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You can't follow someone following you. Lest you be running around in circles indefinitely.</p>",
                player.ConnectionId
            );
            return;
        }

        if (foundPlayer.Config.CanFollow == false)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{foundPlayer.Name} doesn't want to be followed.</p>",
                player.ConnectionId
            );
            return;
        }

        Services.Instance.Writer.WriteLine(
            $"<p>{player.Name} now follows you.</p>",
            foundPlayer.ConnectionId
        );
        Services.Instance.Writer.WriteLine(
            $"<p>You are now following {foundPlayer.Name}.</p>",
            player.ConnectionId
        );

        player.Following = foundPlayer.Name;
        foundPlayer.Followers.Add(player);
    }
}
