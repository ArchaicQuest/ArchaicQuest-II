using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class TellCmd : ICommand
{
    public TellCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"tell"};
        Description = "";
        Usages = new[] {"Type: tell 'player' 'message'"};
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Writer.WriteLine("Tell who?", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(input.ElementAtOrDefault(1)))
        {
            Writer.WriteLine("Tell them what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(2));
        
        var foundPlayer = Cache.GetPlayerCache()
            .FirstOrDefault(x => x.Value.Name.StartsWith(input[1], StringComparison.CurrentCultureIgnoreCase)).Value;

        if (foundPlayer == null)
        {
            Writer.WriteLine("<p>They are not in this realm.</p>", player.ConnectionId);
            return;
        }

        if (foundPlayer == player)
        {
            Writer.WriteLine($"<p>You tell yourself \"{text}\"</p>", player.ConnectionId);
            return;
        }

        if (!foundPlayer.Config.Tells)
        {
            Writer.WriteLine($"<p>They can't hear you.</p>", player.ConnectionId);
            return; 
        }

        player.ReplyTo = foundPlayer.Name;
        foundPlayer.ReplyTo = player.Name;

        Writer.WriteLine($"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", player.ConnectionId);
        UpdateClient.UpdateCommunication(player, $"<p class='say'>You tell {foundPlayer.Name} \"{text}\"</p>", "all");

        Writer.WriteLine($"<p class='say'>{player.Name} tells you \"{text}\"</p>", foundPlayer.ConnectionId);
        UpdateClient.UpdateCommunication(foundPlayer, $"<p class='say'>{player.Name} tells you \"{text}\"</p>", "all");
    }
}