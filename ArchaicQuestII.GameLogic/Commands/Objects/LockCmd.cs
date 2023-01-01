using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class LockCmd : ICommand
{
    private ICommand _commandImplementation;

    public LockCmd(ICore core)
    {
        Aliases = new[] {"lock"};
        Description = "Lock a container or door, You must have the required key to do so.";
        Usages = new[] {"Example: lock chest, lock north"};
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
            CharacterStatus.Status.Resting,
            CharacterStatus.Status.Sitting,
        };
        UserRole = UserRole.Player;
        Core = core;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);
        
        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Lock what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var objToUnlock =
            Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);

        if (objToUnlock == null)
        {

            var doorToUnlock = Helpers.IsExit(target, room);

            if (doorToUnlock != null)
            {
                var playerHasKey = player.Inventory.FirstOrDefault(x =>
                    x.ItemType == Item.Item.ItemTypes.Key && x.KeyId.Equals(doorToUnlock.LockId));
                if (playerHasKey == null)
                {
                    Core.Writer.WriteLine("<p>You don't have the key to lock this.</p>", player.ConnectionId);
                }
                else
                {

                    if (doorToUnlock.Locked)
                    {
                        Core.Writer.WriteLine("<p>It's already locked.</p>", player.ConnectionId);
                        return;
                    }

                    doorToUnlock.Locked = true;
                    Core.Writer.WriteLine("<p>You enter the key and turn it. *CLICK* </p>", player.ConnectionId);
                    Core.Writer.WriteToOthersInRoom($"<p>{player.Name} enters the key into {doorToUnlock.Name} door and turns it. *CLICK* </p>", room, player);

                    return;
                }

                return;
            }

            Core.Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (!objToUnlock.Container.CanLock)
        {
            Core.Writer.WriteLine("<p>You can't lock that.</p>", player.ConnectionId);
            return;
        }

        var hasKey = player.Inventory.FirstOrDefault(x =>
            x.ItemType == Item.Item.ItemTypes.Key && x.KeyId.Equals(objToUnlock.Container.AssociatedKeyId));

        if (hasKey == null)
        {
            Core.Writer.WriteLine("<p>You don't have the key to lock this.</p>", player.ConnectionId);
        }
        else
        {
            if (objToUnlock.Container.IsLocked)
            {
                Core.Writer.WriteLine("<p>It's already locked.</p>", player.ConnectionId);
                return;
            }

            objToUnlock.Container.IsLocked = true;
            Core.Writer.WriteLine("<p>You enter the key and turn it. *CLICK* </p>", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"<p>{player.Name} enters the key into {objToUnlock.Name} and turns it. *CLICK* </p>",
                room, player);
            
        }
    }
}