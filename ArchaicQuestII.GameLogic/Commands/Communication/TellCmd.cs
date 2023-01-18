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
    public TellCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"tell"};
        Description = "Sends a message to player, no matter where they are.";
        Usages = new[] {"Type: tell <player> <message>, Tell Daniel ready to head to the dungeon?"};
            Title = "";
    DeniedStatus = null;
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

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Handler.Client.WriteLine("<p>Tell who?</p>", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Handler.Client.WriteLine("<p>Tell them what?</p>", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(2));
        
        var foundPlayer = Handler.Character.GetPlayerCache()
            .FirstOrDefault(x => x.Value.Name.StartsWith(input[1], StringComparison.CurrentCultureIgnoreCase)).Value;

        if (foundPlayer == null)
        {
            Handler.Client.WriteLine("<p>They are not in this realm.</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer == player)
        {
            Handler.Client.WriteLine($"<p>You tell yourself \"{text}\"</p>", player.ConnectionId);
            return;
        }

        if (!foundPlayer.Config.Tells)
        {
            Handler.Client.WriteLine($"<p>They can't hear you.</p>", player.ConnectionId);
            return; 
        }

        player.ReplyTo = foundPlayer.Name;
        foundPlayer.ReplyTo = player.Name;

        Handler.Client.WriteLine($"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", player.ConnectionId);
        Handler.Client.UpdateCommunication(player, $"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", "all");

        Handler.Client.WriteLine($"<p class='say'>{player.Name} tells you \"{text}\"</p>", foundPlayer.ConnectionId);
        Handler.Client.UpdateCommunication(foundPlayer, $"<p class='say'>{player.Name} tells you \"{text}\"</p>", "all");
    }
}