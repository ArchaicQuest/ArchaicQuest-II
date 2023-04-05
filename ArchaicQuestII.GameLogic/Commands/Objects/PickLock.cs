using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects;

public class PickLockCmd : ICommand
{
    public PickLockCmd(ICore core)
    {
        Aliases = new[] {"picklock", "pick", "pl"};
        Description = "Picks the lock of a locked door or container";
        Usages = new[] {"Type: picklock chest, picklock north"};
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
        var skill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(DefineSkill.LockPick().Name));

        if (skill == null)
        {
            Core.Writer.WriteLine("<p>You don't know how to do that.</p>", player.ConnectionId);
            return;
        }
        
        if (string.IsNullOrEmpty(target))
        {
            Core.Writer.WriteLine("<p>Lock pick what?</p>", player.ConnectionId);
            return;
        }
        
        if (player.Affects.Blind)
        {
            Core.Writer.WriteLine("<p>You are blind and can't see a thing!</p>", player.ConnectionId);
            return;
        }

        var nthItem = Helpers.findNth(target);
        var item = Helpers.findRoomObject(nthItem, room) ?? Helpers.findObjectInInventory(nthItem, player);
        var roomExit = GetRoomExit(target, room);
        
        if (item == null)
        {
            if (roomExit == null)
            {
                Core.Writer.WriteLine("<p>You don't see that here.", player.ConnectionId);
                return;
            }
          
            UnlockDoor(roomExit, skill, room, player);
            return;
        }

        if (item.Container.IsLocked != true)
        {
            Core.Writer.WriteLine($"<p>{item.Name} is already unlocked.", player.ConnectionId);
            return;
        }

        var canDoSkill = Helpers.SkillSuccessCheck(player, DefineSkill.LockPick().Name);
        var difficulty = LockStrength(item.Container.LockDifficulty);
        var chance = DiceBag.Roll(1, 1, 100);
        var successRate = (skill.Proficiency / difficulty) * 10;
        if (!canDoSkill)
        {
            Core.Writer.WriteLine("<p>You fail to pick the lock.</p>", player.ConnectionId);
            Core.Writer.WriteLine(Helpers.SkillLearnMistakes(player, DefineSkill.LockPick().Name, Core.Gain), player.ConnectionId);
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        if (chance <= successRate)
        {
            item.Container.IsLocked = false;
            Core.Writer.WriteLine($"You deftly pick the lock of {item.Name} and it clicks open.", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"{player.Name} deftly picks the lock of {item.Name} and it clicks open.", room, player);
        }
        else
        {
            Core.Writer.WriteLine($"You try to pick the lock of {item.Name}, but it resists your attempts.", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"{player.Name} tries to pick the lock of {item.Name}.", room, player);
        }

        
    }

    private void UnlockDoor(Exit exitDoor, SkillList skill,  Room room, Player player)
    {
        
        if (exitDoor.Locked != true)
        {
            Core.Writer.WriteLine($"<p>{exitDoor.Name} is already unlocked.", player.ConnectionId);
            return;
        }

        var canDoSkill = Helpers.SkillSuccessCheck(player, DefineSkill.LockPick().Name);
        var difficulty = 8; 
        var chance = DiceBag.Roll(1, 1, 100);
        var successRate = (skill.Proficiency / difficulty) * 10;
        if (!canDoSkill)
        {
            Core.Writer.WriteLine("<p>You fail to pick the lock.</p>", player.ConnectionId);
            Core.Writer.WriteLine(Helpers.SkillLearnMistakes(player, DefineSkill.LockPick().Name, Core.Gain), player.ConnectionId);
            player.Status = CharacterStatus.Status.Standing;
            return;
        }

        if (chance <= successRate)
        {
            exitDoor.Locked = false;
            Core.Writer.WriteLine($"You deftly pick the lock of the door and it clicks open.", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"{player.Name} deftly picks the lock of the door and it clicks open.", room, player);
        }
        else
        {
            Core.Writer.WriteLine($"You try to pick the lock of the door, but it resists your attempts.", player.ConnectionId);
            Core.Writer.WriteToOthersInRoom($"{player.Name} tries to pick the lock of the door.", room, player);
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

    private Exit GetRoomExit(string exit, Room room)
    {
        Exit getExitToNextRoom = null;

        switch (exit)
        {
            case "north":
            case "n":
                getExitToNextRoom = room.Exits.North;
                break;
            case "south":
            case "s":
                getExitToNextRoom = room.Exits.South;
                break;
            case "east":
            case "e":
                getExitToNextRoom = room.Exits.East;
                break;
            case "west":
            case "w":
                getExitToNextRoom = room.Exits.West;
                break;
            case "southeast":
            case "se":
                getExitToNextRoom = room.Exits.SouthEast;
                break;
            case "southwest":
            case "sw":
                getExitToNextRoom = room.Exits.SouthWest;
                break;
            case "northeast":
            case "ne":
                getExitToNextRoom = room.Exits.NorthEast;
                break;
            case "northwest":
            case "nw":
                getExitToNextRoom = room.Exits.NorthWest;
                break;
            case "down":
            case "d":
                getExitToNextRoom = room.Exits.Down;
                break;
            case "up":
            case "u":
                getExitToNextRoom = room.Exits.Up;
                break;
        }

        return getExitToNextRoom;
    }
}