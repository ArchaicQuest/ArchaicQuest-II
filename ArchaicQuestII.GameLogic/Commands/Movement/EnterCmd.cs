using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class EnterCmd : ICommand
{
    public EnterCmd(ICore core)
    {
        Aliases = new[] {"enter"};
        Description = "Tries to enter portal.";
        Usages = new[] {"Type: enter portal"};
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
            Core.Writer.WriteLine("<p>You can't do that here.</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room);

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        if (item.ItemType != Item.Item.ItemTypes.Portal)
        {
            Core.Writer.WriteLine("<p>You can't enter that.</p>", player.ConnectionId);
            return;
        }

        Core.Writer.WriteLine($"<p>You {item.Portal.EnterDescription}</p>", player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} {item.Portal.EnterDescription}</p>", room, player);

        var newRoom = Core.Cache.GetRoom(item.Portal.Destination);
        
        Core.RoomActions.RoomChange(player, room, newRoom);
    }
}