using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class CloseCmd : ICommand
{
    public CloseCmd(ICore core)
    {
        Aliases = new[] {"close"};
        Description = "Close is used to close an object or door. For doors type the full name. " +
                      "<br /><br />Example:<br />close chest<br />close north";
        Usages = new[] {"Type: close chest, close south"};
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
            Core.Writer.WriteLine("<p>Close what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);

        if (item != null && item.Container.CanOpen != true)
        {
            Core.Writer.WriteLine($"<p>{item.Name} cannot be closed", player.ConnectionId);
            return;
        }

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        if (!item.Container.IsOpen)
        {
            Core.Writer.WriteLine("<p>It's already closed.", player.ConnectionId);
            return;
        }

        Core.Writer.WriteLine($"<p>You close {item.Name.ToLower()}.</p>", player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} closes {item.Name.ToLower()}</p>", room, player);

        item.Container.IsOpen = false;
        room.Clean = false;
    }
}