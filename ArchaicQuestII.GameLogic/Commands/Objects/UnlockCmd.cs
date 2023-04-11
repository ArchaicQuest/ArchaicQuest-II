using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class UnlockCmd : ICommand
{
    public UnlockCmd()
    {
        Aliases = new[] { "unlock" };
        Description = "Unlock a container or door, You must have the required key to do so.";
        Usages = new[] { "Type: unlock north" };
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
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Unlock what?</p>", player.ConnectionId);
            return;
        }

        if (player.Affects.Blind)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You are blind and can't see a thing!</p>",
                player.ConnectionId
            );
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
                var playerDoorKey = player.Inventory.FirstOrDefault(
                    x =>
                        x.ItemType == Item.Item.ItemTypes.Key && x.KeyId.Equals(doorToUnlock.LockId)
                );
                if (playerDoorKey == null)
                {
                    Services.Instance.Writer.WriteLine(
                        "<p>You don't have the key to unlock this.</p>",
                        player.ConnectionId
                    );
                }
                else
                {
                    if (!doorToUnlock.Locked)
                    {
                        Services.Instance.Writer.WriteLine(
                            "<p>It's already unlocked.</p>",
                            player.ConnectionId
                        );
                        return;
                    }

                    doorToUnlock.Locked = false;
                    Services.Instance.UpdateClient.PlaySound("unlock", player);
                    // play sound for others in the room
                    foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
                    {
                        Services.Instance.UpdateClient.PlaySound("unlock", pc);
                    }
                    Services.Instance.Writer.WriteLine(
                        "<p>You enter the key and turn it. *CLICK* </p>",
                        player.ConnectionId
                    );
                    Services.Instance.Writer.WriteToOthersInRoom(
                        $"<p>{player.Name} enters the key and turns it. *CLICK* </p>",
                        room,
                        player
                    );

                    if (playerDoorKey.Uses == -1)
                        return;

                    playerDoorKey.Uses--;
                    if (playerDoorKey.Uses == 0)
                    {
                        player.Inventory.Remove(playerDoorKey);
                        Services.Instance.Writer.WriteLine(
                            "<p>As you go to put the key away it crumbles to dust in your hand.</p>",
                            player.ConnectionId
                        );
                    }

                    return;
                }
                return;
            }

            Services.Instance.Writer.WriteLine(
                "<p>You don't see that here.</p>",
                player.ConnectionId
            );
            return;
        }

        if (!objToUnlock.Container.CanLock)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You can't unlock that.</p>",
                player.ConnectionId
            );
            return;
        }

        var playerContainerKey = player.Inventory.FirstOrDefault(
            x =>
                x.ItemType == Item.Item.ItemTypes.Key
                && x.KeyId.Equals(objToUnlock.Container.AssociatedKeyId)
        );

        if (playerContainerKey == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't have the key to unlock this.</p>",
                player.ConnectionId
            );
        }
        else
        {
            if (!objToUnlock.Container.IsLocked)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>It's already unlocked.</p>",
                    player.ConnectionId
                );
                return;
            }

            objToUnlock.Container.IsLocked = false;
            Services.Instance.Writer.WriteLine(
                "<p>You enter the key and turn it. *CLICK* </p>",
                player.ConnectionId
            );
            Services.Instance.Writer.WriteToOthersInRoom(
                $"<p>{player.Name} enters the key into {objToUnlock.Name} and turns it. *CLICK* </p>",
                room,
                player
            );

            if (playerContainerKey.Uses == -1)
                return;

            playerContainerKey.Uses--;

            if (playerContainerKey.Uses == 0)
            {
                player.Inventory.Remove(playerContainerKey);
                Services.Instance.Writer.WriteLine(
                    "<p>As you go to put the key away it crumbles to dust in your hand.</p>",
                    player.ConnectionId
                );
            }
        }
    }
}
