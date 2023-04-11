using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class ExamineCmd : ICommand
{
    public ExamineCmd()
    {
        Aliases = new[] { "examine" };
        Description =
            "You examine an object, showing you a more detailed description if you want it. "
            + "This may provide more information or clues about your surroundings, there are plenty of hidden and secret places"
            + "in ArchaicQuest.";
        Usages = new[] { "Type: examine flag" };
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
            Services.Instance.Writer.WriteLine("<p>Examine what?</p>", player.ConnectionId);
            return;
        }

        var nthTarget = Helpers.findNth(target);
        var item =
            Helpers.findRoomObject(nthTarget, room) ?? player.FindObjectInInventory(nthTarget);

        if (item == null && room.RoomObjects.Count >= 1 && room.RoomObjects[0].Name != null)
        {
            var roomObjects = room.RoomObjects.FirstOrDefault(
                x => x.Name.Contains(target, StringComparison.CurrentCultureIgnoreCase)
            );

            Services.Instance.Writer.WriteLine(
                $"<p class='{(!player.CanSee(room) ? "room-dark" : "")}'>{roomObjects.Examine ?? roomObjects.Look}",
                player.ConnectionId
            );

            return;
        }

        if (item == null)
        {
            Services.Instance.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
            return;
        }

        var examMessage =
            item.Description.Exam == "You don't see anything special."
                ? $"On closer inspection you don't see anything special to note to what you already see. {item.Description.Look}"
                : item.Description.Exam;

        Services.Instance.Writer.WriteLine(
            $"<p class='{(!player.CanSee(room) ? "room-dark" : "")}'>{examMessage}",
            player.ConnectionId
        );
        Services.Instance.Writer.WriteToOthersInRoom(
            $"<p>{player.Name} examines {item.Name.ToLower()}.</p>",
            room,
            player
        );
    }
}
