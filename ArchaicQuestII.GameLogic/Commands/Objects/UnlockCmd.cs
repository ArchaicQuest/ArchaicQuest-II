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
            Services.Instance.Writer.WriteLine("<p>Unlock what?</p>", player);
            return;
        }

        if (player.Affects.Blind)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You are blind and can't see a thing!</p>",
                player
            );
            return;
        }

        var nthItem = Helpers.findNth(target);
        var objToUnlock =
            Helpers.findRoomObject(nthItem, room) ?? player.FindObjectInInventory(nthItem);

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
                        player
                    );
                }
                else
                {
                    if (!doorToUnlock.Locked)
                    {
                        Services.Instance.Writer.WriteLine("<p>It's already unlocked.</p>", player);
                        return;
                    }
                    
                    var oppositeRoom =
                        Services.Instance.Cache.GetRoom(
                            $"{doorToUnlock.AreaId}{doorToUnlock.Coords.X}{doorToUnlock.Coords.Y}{doorToUnlock.Coords.Z}");

                    if (oppositeRoom != null)
                    {
                        var oppositeExit = Helpers.IsExit(GetOppositeExit(target), oppositeRoom);

                        if (oppositeExit != null)
                        {
                            oppositeExit.Locked = false;
                        }
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
                        player
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
                            player
                        );
                    }

                    return;
                }
                return;
            }

            Services.Instance.Writer.WriteLine("<p>You don't see that here.</p>", player);
            return;
        }

        if (!objToUnlock.Container.CanLock)
        {
            Services.Instance.Writer.WriteLine("<p>You can't unlock that.</p>", player);
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
                player
            );
        }
        else
        {
            if (!objToUnlock.Container.IsLocked)
            {
                Services.Instance.Writer.WriteLine("<p>It's already unlocked.</p>", player);
                return;
            }

            objToUnlock.Container.IsLocked = false;
            Services.Instance.Writer.WriteLine(
                "<p>You enter the key and turn it. *CLICK* </p>",
                player
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
                    player
                );
            }
        }
    }
    
      
    private string GetOppositeExit(string direction)
    {
        switch (direction)
        {
            case "north":
            case "n":
                return "s";
            case "south":
            case "s":
                return "n";
            case "east":
            case "e":
                return "w";
            case "west":
            case "w":
                return "e";
            case "southeast":
            case "se":
                return "nw";
            case "southwest":
            case "sw":
                return "ne";
            case "northeast":
            case "ne":
                return "sw";
            case "northwest":
            case "nw":
                return "se";
            case "down":
            case "d":
                return "u";
            case "up":
            case "u":
                return "d";
            default:
                return "";
        }
    }
}
