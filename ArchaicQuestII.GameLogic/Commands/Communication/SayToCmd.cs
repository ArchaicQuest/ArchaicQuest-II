using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Communication;

public class SayToCmd : ICommand
{
    public SayToCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"sayto"};
        Description = "Says something to a player.";
        Usages = new[] {"Type: sayto john 'what ever you want'"};
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
        
        var text = string.Join(" ", input.Skip(2));

        //find target
        var sayTo = room.Players.FirstOrDefault(x => x.Name.StartsWith(input[1], StringComparison.CurrentCultureIgnoreCase));

        if (sayTo == null)
        {
            Writer.WriteLine("<p>They are not here.</p>", player.ConnectionId);
            return;
        }

        Writer.WriteLine($"<p class='say'>You say to {sayTo.Name}, {text}</p>", player.ConnectionId);
        UpdateClient.UpdateCommunication(player, $"<p class='say'>You say to {sayTo.Name}, {text}</p>", "room");
        
        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            if (pc.Name == sayTo.Name)
            {
                Writer.WriteLine($"<p class='say'>{player.Name} says to you, {text}</p>", pc.ConnectionId);
                UpdateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says to you, {text}</p>", "room");
            }
            else
            {
                Writer.WriteLine($"<p class='say'>{player.Name} says to {sayTo.Name}, {text}</p>", pc.ConnectionId);
                UpdateClient.UpdateCommunication(pc, $"<p class='say'>{player.Name} says to {sayTo.Name}, {text}</p>", "room");
            }
        }

    }
}