using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class EnterCmd : ICommand
{
    public EnterCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"enter"};
        Description = "Tries to enter portal.";
        Usages = new[] {"Type: enter portal"};
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
            Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room);

        if (item == null)
        {
            Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (item.ItemType != Item.Item.ItemTypes.Portal)
        {
            Writer.WriteLine("<p>You can't enter that.</p>", player.ConnectionId);
            return;
        }

        foreach (var pc in room.Players)
        {
            if (player.Name == pc.Name)
            {
                Writer.WriteLine($"<p>You {item.Portal.EnterDescription}</p>", player.ConnectionId);
                continue;
            }
            Writer.WriteLine($"<p>{player.Name} {item.Portal.EnterDescription}</p>", pc.ConnectionId);
        }

        var newRoom = Cache.GetRoom(item.Portal.Destination);
        
        RoomActions.RoomChange(player, room, newRoom);

        foreach (var pc in newRoom.Players.Where(pc => player.Name != pc.Name))
        {
            Writer.WriteLine($"<p>{player.Name} {item.Portal.EnterDescriptionRoom}</p>", pc.ConnectionId);
        }
    }
}