using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class ExamineCmd : ICommand
{
    public ExamineCmd(ICore core)
    {
        Aliases = new[] {"examine"};
        Description = "You examine an object.";
        Usages = new[] {"Type: examine flag"};
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
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Examine what?</p>", player.ConnectionId);
            return;
        }
        
        var nthTarget = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthTarget, room) ?? Helpers.findObjectInInventory(nthTarget, player);
        var isDark = Core.RoomActions.RoomIsDark(room, player);
            
        if (item == null && room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
        {
            var roomObjects = room.RoomObjects.FirstOrDefault(x =>
                x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase));

            Core.Writer.WriteLine(
                $"<p class='{(isDark ? "room-dark" : "")}'>{roomObjects.Examine ?? roomObjects.Look}",
                player.ConnectionId);

            return;
        }

        if (item == null)
        {
            Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        var examMessage = item.Description.Exam == "You don't see anything special."
            ? $"On closer inspection you don't see anything special to note to what you already see. {item.Description.Look}"
            : item.Description.Exam;
        Core.Writer.WriteLine($"<p class='{(isDark ? "room-dark" : "")}'>{examMessage}", player.ConnectionId);

        foreach (var pc in room.Players.Where(pc => pc.Name != player.Name))
        {
            Core.Writer.WriteLine($"<p>{player.Name} examines {item.Name.ToLower()}.</p>", pc.ConnectionId);
        }
    }
}