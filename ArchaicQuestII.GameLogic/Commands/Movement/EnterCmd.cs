using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class EnterCmd : ICommand
{
    public EnterCmd()
    {
        Aliases = new[] { "enter" };
        Description =
            "If you happen to find a portal or hole in the wall or some other clear exit that is not of "
            + "the usual cardinal directions the command to use to take the exit is enter. <br /><br />Example:<br />enter portal<br />enter hole";
        Usages = new[] { "Type: enter portal" };
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
            CharacterStatus.Status.Resting
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
            CoreHandler.Instance.Writer.WriteLine(
                "<p>You can't do that here.</p>",
                player.ConnectionId
            );
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room);

        if (item == null)
        {
            CoreHandler.Instance.Writer.WriteLine(
                "<p>You don't see that here.</p>",
                player.ConnectionId
            );
            return;
        }

        if (item.ItemType != Item.Item.ItemTypes.Portal)
        {
            CoreHandler.Instance.Writer.WriteLine(
                "<p>You can't enter that.</p>",
                player.ConnectionId
            );
            return;
        }

        CoreHandler.Instance.Writer.WriteLine(
            $"<p>You {item.Portal.EnterDescription}</p>",
            player.ConnectionId
        );
        CoreHandler.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} {item.Portal.EnterDescription}</p>",
            room,
            player
        );

        var newRoom = CoreHandler.Instance.Cache.GetRoom(item.Portal.Destination);

        CoreHandler.Instance.RoomActions.RoomChange(player, room, newRoom, false);
    }
}
