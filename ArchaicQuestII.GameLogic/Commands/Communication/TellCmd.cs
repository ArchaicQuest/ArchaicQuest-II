using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class TellCmd : ICommand
{
    public TellCmd()
    {
        Aliases = new[] { "tell" };
        Description = "Sends a message to player, no matter where they are.";
        Usages = new[]
        {
            "Type: tell <player> <message>, Tell Daniel ready to head to the dungeon?"
        };
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Services.Instance.Writer.WriteLine("<p>Tell who?</p>", player);
            return;
        }

        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Services.Instance.Writer.WriteLine("<p>Tell them what?</p>", player);
            return;
        }

        var text = string.Join(" ", input.Skip(2));

        var foundPlayer = Services.Instance.Cache
            .GetPlayerCache()
            .FirstOrDefault(
                x => x.Value.Name.StartsWith(input[1], StringComparison.CurrentCultureIgnoreCase)
            )
            .Value;

        if (foundPlayer == null)
        {
            Services.Instance.Writer.WriteLine("<p>They are not in this realm.</p>", player);
            return;
        }

        if (foundPlayer == player)
        {
            Services.Instance.Writer.WriteLine($"<p>You tell yourself \"{text}\"</p>", player);
            return;
        }

        if (!foundPlayer.Config.Tells)
        {
            Services.Instance.Writer.WriteLine($"<p>They can't hear you.</p>", player);
            return;
        }

        player.ReplyTo = foundPlayer.Name;
        foundPlayer.ReplyTo = player.Name;

        Services.Instance.Writer.WriteLine(
            $"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>",
            player
        );
        Services.Instance.UpdateClient.UpdateCommunication(
            player,
            $"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>",
            "all"
        );

        Services.Instance.Writer.WriteLine(
            $"<p class='say'>{player.Name} tells you \"{text}\"</p>",
            foundPlayer
        );
        Services.Instance.UpdateClient.UpdateCommunication(
            foundPlayer,
            $"<p class='say'>{player.Name} tells you \"{text}\"</p>",
            "all"
        );
    }
}
