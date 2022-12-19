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
    public TellCmd(ICore core)
    {
        Aliases = new[] {"tell"};
        Description = "Sends a message to player, no matter where they are.";
        Usages = new[] {"Type: tell 'player' 'message'"};
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
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("Tell who?", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Core.Writer.WriteLine("Tell them what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(2));
        
        var foundPlayer = Core.Cache.GetPlayerCache()
            .FirstOrDefault(x => x.Value.Name.StartsWith(input[1], StringComparison.CurrentCultureIgnoreCase)).Value;

        if (foundPlayer == null)
        {
            Core.Writer.WriteLine("<p>They are not in this realm.</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer == player)
        {
            Core.Writer.WriteLine($"<p>You tell yourself \"{text}\"</p>", player.ConnectionId);
            return;
        }

        if (!foundPlayer.Config.Tells)
        {
            Core.Writer.WriteLine($"<p>They can't hear you.</p>", player.ConnectionId);
            return; 
        }

        player.ReplyTo = foundPlayer.Name;
        foundPlayer.ReplyTo = player.Name;

        Core.Writer.WriteLine($"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", player.ConnectionId);
        Core.UpdateClient.UpdateCommunication(player, $"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", "all");

        Core.Writer.WriteLine($"<p class='say'>{player.Name} tells you \"{text}\"</p>", foundPlayer.ConnectionId);
        Core.UpdateClient.UpdateCommunication(foundPlayer, $"<p class='say'>{player.Name} tells you \"{text}\"</p>", "all");
    }
}