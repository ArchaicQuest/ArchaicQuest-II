using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class UnlockCmd : ICommand
{
    public UnlockCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"unlock"};
        Description = "You try to unlock something.";
        Usages = new[] {"Type: unlock north"};
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
            Writer.WriteLine("<p>Unlock what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var objToUnlock = Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);

        if (objToUnlock == null)
        {
            var doorToUnlock = Helpers.IsExit(target, room);

            if (doorToUnlock != null)
            {
                var playerHasKey = player.Inventory.FirstOrDefault(x =>
                    x.ItemType == Item.Item.ItemTypes.Key && x.KeyId.Equals(doorToUnlock.LockId));
                if (playerHasKey == null)
                {
                    Writer.WriteLine("<p>You don't have the key to unlock this.</p>", player.ConnectionId);
                }
                else
                {

                    if (!doorToUnlock.Locked)
                    {
                        Writer.WriteLine("<p>It's already unlocked.</p>", player.ConnectionId);
                        return;
                    }

                    doorToUnlock.Locked = false;
                    Writer.WriteLine("<p>You enter the key and turn it. *CLICK* </p>", player.ConnectionId);

                    foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        Writer.WriteLine($"<p>{pc.Name} enters the key and turns it. *CLICK* </p>", pc.ConnectionId);
                    }

                    return;
                }
                return;
            }

            Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (!objToUnlock.Container.CanLock)
        {
            Writer.WriteLine("<p>You can't unlock that.</p>", player.ConnectionId);
            return;
        }

        var hasKey = player.Inventory.FirstOrDefault(x =>
            x.ItemType == Item.Item.ItemTypes.Key && x.KeyId.Equals(objToUnlock.Container.AssociatedKeyId));

        if (hasKey == null)
        {
            Writer.WriteLine("<p>You don't have the key to unlock this.</p>", player.ConnectionId);
        }
        else
        {
            if (!objToUnlock.Container.IsLocked)
            {
                Writer.WriteLine("<p>It's already unlocked.</p>", player.ConnectionId);
                return;
            }

            objToUnlock.Container.IsLocked = false;
            Writer.WriteLine("<p>You enter the key and turn it. *CLICK* </p>", player.ConnectionId);

            foreach (var pc in room.Players.Where(pc => !pc.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                Writer.WriteLine($"<p>{pc.Name} enters the key into {objToUnlock.Name} and turns it. *CLICK* </p>",
                    pc.ConnectionId);
            }
        }
    }
}