using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class TasteCmd : ICommand
{
    public TasteCmd(ICore core)
    {
        Aliases = new[] {"taste"};
        Description = "You can taste an object, and find out how it tastes.";
        Usages = new[] {"Type: taste cake"};
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
            Core.Writer.WriteLine("<p>Taste what?</p>", player.ConnectionId);
            return;
        }
        
        var nthTarget = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            
        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't see that here.</p>", player.ConnectionId);
            return;
        }

        var isDark = Core.RoomActions.RoomIsDark(player, room);

        Core.Writer.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Taste}</p>",
            player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"<p>{player.Name} tastes {item.Name.ToLower()}.</p>", room, player);
    }
}