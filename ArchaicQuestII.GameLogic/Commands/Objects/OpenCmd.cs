using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class OpenCmd : ICommand
{
    public OpenCmd(ICore core)
    {
        Aliases = new[] {"open"};
        Description = "Open is used to open an object or door. For doors type the full name. " +
                      "<br /><br />Example:<br />open chest<br />open north";
        Usages = new[] {"Type: open chest, open north"};
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
            Core.Writer.WriteLine("<p>Open what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
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
                Core.Writer.WriteLine($"<p>You open the door {isExit.Name}.", player.ConnectionId);
                Core.UpdateClient.PlaySound("door", player);
                // play sound for others in the room
                foreach (var pc in room.Players.Where(pc => pc.Id != player.Id))
                {
                    Core.UpdateClient.PlaySound("door", pc);
                }
                return;
            }

            if (isExit.Locked)
            {
                Core.Writer.WriteLine("<p>You try to open it but it's locked.", player.ConnectionId);
                return;
            }
        }

        if (item != null && item.Container.CanOpen != true)
        {
            Core.Writer.WriteLine($"<p>{item.Name} cannot be opened", player.ConnectionId);
            return;
        }

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        if (item.Container.IsOpen)
        {
            Core.Writer.WriteLine("<p>It's already open.</p>", player.ConnectionId);
            return;
        }

        Core.Writer.WriteLine($"<p>You open {item.Name.ToLower()}.</p>", player.ConnectionId);
        
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} opens {item.Name.ToLower()}.</p>", room, player);

        item.Container.IsOpen = true;
        room.Clean = false;
    }
}