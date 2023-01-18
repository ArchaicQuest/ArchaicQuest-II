using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class SmellCmd : ICommand
{
    public SmellCmd(ICoreHandler coreHandler)
    {
        Aliases = new[] {"smell"};
        Description = "You can smell an object to find out about it's smell";
        Usages = new[] {"Type: smell flower"};
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

        Handler = coreHandler;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICoreHandler Handler { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Handler.Client.WriteLine("<p>Smell what?</p>", player.ConnectionId);
            return;
        }
        
        var nthTarget = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
            
        if (item == null)
        {
            Handler.Client.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        var isDark = Handler.World.RoomIsDark(player, room);

        Handler.Client.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{item.Description.Smell}",
            player.ConnectionId);
        Handler.Client.WriteToOthersInRoom($"<p>{player.Name} smells {item.Name.ToLower()}.</p>", room, player);
    }
}