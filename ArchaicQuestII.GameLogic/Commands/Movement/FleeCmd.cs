using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class FleeCmd : ICommand
{
    public FleeCmd()
    {
        Aliases = new[] { "flee" };
        Description =
            "Randomly moves your character out of the room during combat. "
            + "If the room has a door, make sure it's open when you flee otherwise you will be unable to flee until it's open";
        Usages = new[] { "Type: north" };
        Title = "";
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
    }

    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public string Title { get; }
    public CharacterStatus.Status[] DeniedStatus { get; }
    public UserRole UserRole { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (player.Status != CharacterStatus.Status.Fighting)
        {
            Services.Instance.Writer.WriteLine("<p>You're not in a fight.</p>", player);
            return;
        }

        if (
            room.Exits.Down == null
            && room.Exits.Up == null
            && room.Exits.NorthWest == null
            && room.Exits.North == null
            && room.Exits.NorthEast == null
            && room.Exits.East == null
            && room.Exits.SouthEast == null
            && room.Exits.South == null
            && room.Exits.SouthWest == null
            && room.Exits.West == null
        )
        {
            Services.Instance.Writer.WriteLine("<p>You have no where to go!</p>", player);
            return;
        }

        var validExits = new List<Exit>();

        if (room.Exits.North != null && !room.Exits.North.Closed)
        {
            validExits.Add(room.Exits.North);
        }
        if (room.Exits.NorthEast != null && !room.Exits.NorthEast.Closed)
        {
            validExits.Add(room.Exits.NorthEast);
        }
        if (room.Exits.NorthWest != null && !room.Exits.NorthWest.Closed)
        {
            validExits.Add(room.Exits.NorthWest);
        }
        if (room.Exits.East != null && !room.Exits.East.Closed)
        {
            validExits.Add(room.Exits.East);
        }
        if (room.Exits.SouthEast != null && !room.Exits.SouthEast.Closed)
        {
            validExits.Add(room.Exits.SouthEast);
        }
        if (room.Exits.South != null && !room.Exits.South.Closed)
        {
            validExits.Add(room.Exits.South);
        }
        if (room.Exits.SouthWest != null && !room.Exits.SouthWest.Closed)
        {
            validExits.Add(room.Exits.SouthWest);
        }
        if (room.Exits.West != null && !room.Exits.West.Closed)
        {
            validExits.Add(room.Exits.West);
        }
        if (room.Exits.Up != null && !room.Exits.Up.Closed)
        {
            validExits.Add(room.Exits.Up);
        }
        if (room.Exits.Down != null && !room.Exits.Down.Closed)
        {
            validExits.Add(room.Exits.Down);
        }

        if (validExits.Count == 0)
        {
            Services.Instance.Writer.WriteLine("<p>You have no where to go!</p>", player);
            return;
        }

        var getExitIndex = DiceBag.Roll(1, 0, validExits.Count - 1);

        player.Status = CharacterStatus.Status.Standing;

        Services.Instance.Cache.RemoveCharFromCombat(player.Id.ToString());

        foreach (var mob in room.Mobs.Where(mob => mob.Target == player.Name))
        {
            mob.Status = CharacterStatus.Status.Standing;
            Services.Instance.Cache.RemoveCharFromCombat(mob.Id.ToString());
        }

        Services.Instance.Cache
            .GetCommand(validExits[getExitIndex].Name.ToLower())
            .Execute(player, room, new[] { validExits[getExitIndex].Name.ToLower(), "flee" });

        // costs 15% of your moves to flee
        player.Attributes.Attribute[EffectLocation.Moves] -=
            (100 * 15) / player.MaxAttributes.Attribute[EffectLocation.Moves];

        Services.Instance.UpdateClient.UpdateMoves(player);
    }
}
