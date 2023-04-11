using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class PickLockCmd : ICommand
{
    public PickLockCmd()
    {
        Aliases = new[] { "picklock", "pick", "pl", "lockpick" };
        Description = "Picks the lock of a locked door or container";
        Usages = new[] { "Type: picklock chest, picklock north" };
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
        var skill = player.Skills.FirstOrDefault(x => x.Name == SkillName.Lockpick);

        if (skill == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>You don't know how to do that.</p>",
                player.ConnectionId
            );
            return;
        }

        if (string.IsNullOrEmpty(target))
        {
            Services.Instance.Writer.WriteLine("<p>Lock pick what?</p>", player.ConnectionId);
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
        var item =
            Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);
        var roomExit = Services.Instance.RoomActions.GetRoomExit(target, room);

        if (item == null)
        {
            if (roomExit == null)
            {
                Services.Instance.Writer.WriteLine(
                    "<p>You don't see that here.",
                    player.ConnectionId
                );
                return;
            }

            UnlockDoor(roomExit, skill, room, player);
            return;
        }

        if (item.Container.IsLocked != true)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{item.Name} is already unlocked.",
                player.ConnectionId
            );
            return;
        }

        var difficulty = LockStrength(item.Container.LockDifficulty);
        var chance = DiceBag.Roll(1, 1, 100);
        var successRate = (skill.Proficiency / difficulty) * 10;

        if (chance <= successRate)
        {
            item.Container.IsLocked = false;
            Services.Instance.Writer.WriteLine(
                $"You deftly pick the lock of {item.Name.FirstCharacterToLower()} and it clicks open.",
                player.ConnectionId
            );
            Services.Instance.UpdateClient.PlaySound("unlock", player);
            Services.Instance.Writer.WriteLine($"*Click*", player.ConnectionId);
            Services.Instance.Writer.WriteToOthersInRoom(
                $"{player.Name} deftly picks the lock of {item.Name} and it clicks open.",
                room,
                player
            );
        }
        else
        {
            Services.Instance.Writer.WriteLine(
                $"You try to pick the lock of {item.Name.FirstCharacterToLower()}, but it resists your attempts.",
                player.ConnectionId
            );
            Services.Instance.Writer.WriteToOthersInRoom(
                $"{player.Name} tries to pick the lock of {item.Name}.",
                room,
                player
            );
            player.FailedSkill(SkillName.Lockpick, out var message);
            Services.Instance.Writer.WriteLine(message, player.ConnectionId);
            player.Status = CharacterStatus.Status.Standing;
        }
    }

    private void UnlockDoor(Exit exitDoor, SkillList skill, Room room, Player player)
    {
        if (exitDoor.Locked != true)
        {
            Services.Instance.Writer.WriteLine(
                $"<p>{exitDoor.Name} is already unlocked.",
                player.ConnectionId
            );
            return;
        }

        var difficulty = 8;
        var chance = DiceBag.Roll(1, 1, 100);
        var successRate = (skill.Proficiency / difficulty) * 10;

        if (chance <= successRate)
        {
            exitDoor.Locked = false;
            Services.Instance.Writer.WriteLine(
                $"You deftly pick the lock of the door and it clicks open.",
                player.ConnectionId
            );
            Services.Instance.UpdateClient.PlaySound("unlock", player);
            Services.Instance.Writer.WriteLine($"*Click*", player.ConnectionId);
            Services.Instance.Writer.WriteToOthersInRoom(
                $"{player.Name} deftly picks the lock of the door and it clicks open.",
                room,
                player
            );
        }
        else
        {
            Services.Instance.Writer.WriteLine(
                $"You try to pick the lock of the door, but it resists your attempts.",
                player.ConnectionId
            );
            Services.Instance.Writer.WriteToOthersInRoom(
                $"{player.Name} tries to pick the lock of the door.",
                room,
                player
            );
            player.FailedSkill(SkillName.Lockpick, out var message);
            Services.Instance.Writer.WriteLine(message, player.ConnectionId);
            player.Status = CharacterStatus.Status.Standing;
        }
    }

    private int LockStrength(Item.Item.LockStrength lockStrength)
    {
        switch (lockStrength)
        {
            case Item.Item.LockStrength.Simple:
                return 4;
            case Item.Item.LockStrength.Easy:
                return 6;
            case Item.Item.LockStrength.Medium:
                return 8;
            case Item.Item.LockStrength.Hard:
                return 10;
            case Item.Item.LockStrength.Impossible:
                return 12;
            default:
                return 24;
        }
    }
}
