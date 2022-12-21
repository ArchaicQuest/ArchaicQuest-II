using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class FleeCmd : ICommand
{
    public FleeCmd(ICore core)
    {
        Aliases = new[] {"flee"};
        Description = "Randomly moves your character out of the room during combat.";
        Usages = new[] {"Type: north"};
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
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
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }
    public ICore Core { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (player.Status != CharacterStatus.Status.Fighting)
        {
            Core.Writer.WriteLine("<p>You're not in a fight.</p>", player.ConnectionId);
            return;
        }

        if (room.Exits.Down == null &&
            room.Exits.Up == null &&
            room.Exits.NorthWest == null &&
            room.Exits.North == null &&
            room.Exits.NorthEast == null &&
            room.Exits.East == null &&
            room.Exits.SouthEast == null &&
            room.Exits.South == null &&
            room.Exits.SouthWest == null &&
            room.Exits.West == null)
        {
            Core.Writer.WriteLine("<p>You have no where to go!</p>", player.ConnectionId);
            return;
        }

        var validExits = new List<Exit>();

        if (room.Exits.North != null)
        {
            validExits.Add(room.Exits.North);
        }
        if (room.Exits.NorthEast != null)
        {
            validExits.Add(room.Exits.NorthEast);
        }
        if (room.Exits.NorthWest != null)
        {
            validExits.Add(room.Exits.NorthWest);
        }
        if (room.Exits.East != null)
        {
            validExits.Add(room.Exits.East);
        }
        if (room.Exits.SouthEast != null)
        {
            validExits.Add(room.Exits.SouthEast);
        }
        if (room.Exits.South != null)
        {
            validExits.Add(room.Exits.South);
        }
        if (room.Exits.SouthWest != null)
        {
            validExits.Add(room.Exits.SouthWest);
        }
        if (room.Exits.West != null)
        {
            validExits.Add(room.Exits.West);
        }
        if (room.Exits.Up != null)
        {
            validExits.Add(room.Exits.Up);
        }
        if (room.Exits.Down != null)
        {
            validExits.Add(room.Exits.Down);
        }

        var getExitIndex = Core.Dice.Roll(1, 0, validExits.Count - 1);

        player.Status = CharacterStatus.Status.Standing;
        
        Core.Cache.RemoveCharFromCombat(player.Id.ToString());

        foreach (var mob in room.Mobs.Where(mob => mob.Target == player.Name))
        {
            mob.Status = CharacterStatus.Status.Standing;
            Core.Cache.RemoveCharFromCombat(mob.Id.ToString());
        }

        var randomFleeMsg = new List<string>
        {
            $"{player.Name} turns and flees",
            $"{player.Name} screams and runs for their life",
            $"{player.Name} ducks and rolls before running away",
            $"{player.Name} retreats from combat."
        };

        var fleeString = randomFleeMsg[Core.Dice.Roll(1, 0, randomFleeMsg.Count)];

        Core.Writer.WriteLine($"<p>You flee {validExits[getExitIndex].Name}.</p>",  player.ConnectionId);
        Core.Writer.WriteToOthersInRoom($"{fleeString}.", room, player);
        Core.Cache.GetCommand(validExits[getExitIndex].Name).Execute(player, room, new[]{validExits[getExitIndex].Name});
    }
}