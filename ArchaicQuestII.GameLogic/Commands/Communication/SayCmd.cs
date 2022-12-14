using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class SayCmd : ICommand
{
    public SayCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"say"};
        Description = "Say something to the room.";
        Usages = new[] {"Type: say 'what ever you want'"};
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
            Writer.WriteLine("Say what?", player.ConnectionId);
            return;
        }
        
        var text = string.Join(" ", input.Skip(1));
        
        Writer.WriteLine($"<p class='say'>You say {text}</p>", player.ConnectionId);
        
        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Writer.WriteLine($"<p class='say'>{player.Name} says {text}</p>", pc.ConnectionId);
            UpdateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says {text}</p>", "room");
        }
        UpdateClient.UpdateCommunication(player, $"<p class='say'>You say {text}</p>", "room");
    }
}