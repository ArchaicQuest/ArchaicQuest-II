using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class OpenCmd : ICommand
{
    public OpenCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"open"};
        Description = "Your open a door or chest.";
        Usages = new[] {"Type: open north"};
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
        var target = input.ElementAtOrDefault(1);
        
        if (string.IsNullOrEmpty(target))
        {
            Writer.WriteLine("<p>Open what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);
        var isExit = Helpers.IsExit(target, room);

        if (isExit != null)
        {
            if (!isExit.Locked)
            {
                isExit.Closed = false;
                Writer.WriteLine($"<p>You open the door {isExit.Name}.", player.ConnectionId);
                return;
            }

            if (isExit.Locked)
            {
                Writer.WriteLine($"<p>You try to open it but it's locked.", player.ConnectionId);
                return;
            }
        }

        if (item != null && item.Container.CanOpen != true)
        {
            Writer.WriteLine($"<p>{item.Name} cannot be opened", player.ConnectionId);
            return;
        }

        if (item == null)
        {
            Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        if (item.Container.IsOpen)
        {
            Writer.WriteLine("<p>It's already open.", player.ConnectionId);
            return;
        }

        Writer.WriteLine($"<p>You open {item.Name.ToLower()}.</p>", player.ConnectionId);

        item.Container.IsOpen = true;
        
        foreach (var obj in room.Players.Where(obj => obj.Name != player.Name))
        {
            Writer.WriteLine($"<p>{player.Name} opens {item.Name.ToLower()}</p>", obj.ConnectionId);
        }
        room.Clean = false;
    }
}