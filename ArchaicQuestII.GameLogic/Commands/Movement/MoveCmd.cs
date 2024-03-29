using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Movement;

public class MoveCmd : ICommand
{
    public MoveCmd()
    {
        Title = "Movement";
        Aliases = new[]
        {
            "north",
            "n",
            "south",
            "s",
            "east",
            "e",
            "west",
            "w",
            "southeast",
            "se",
            "southwest",
            "sw",
            "northeast",
            "ne",
            "northwest",
            "nw",
            "up",
            "u",
            "down",
            "d"
        };
        Description =
            @"<pre><p>To move around you type in one of the following commands: {yellow}north{/}, {yellow}east{/}, {yellow}south{/}, {yellow}west{/}, {yellow}up{/}, {yellow}down{/} {yellow}northeast{/}, {yellow}southeast{/}, {yellow}southwest{/}, and {yellow}northwest{/}. These commands may also be shortened to:  {yellow}n{/}, {yellow}e{/}, {yellow}s{/}, {yellow}w{/}, {yellow}u{/}, {yellow}d{/}, {yellow}ne{/}, {yellow}se{/}, {yellow}sw{/}, and {yellow}nw{/}.</p><p>Moving consumes movement points, shown in the green stat bar. Stats Replenish slowly but can be sped up by using the sit, rest, or sleep commands. When finished recovering you will need to wake or stand before you can move again.</p></pre>";
        Usages = new[]
        {
            "Type: north or n for short to move north. Valid Directions: n,e,s,w,u,d,nw,ne,se,sw"
        };
        DeniedStatus = new[]
        {
            CharacterStatus.Status.Busy,
            CharacterStatus.Status.Dead,
            CharacterStatus.Status.Fighting,
            CharacterStatus.Status.Ghost,
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
        if (CharacterCanMove(player) == false)
        {
            Services.Instance.Writer.WriteLine("<p>You are too exhausted to move.</p>", player);
            return;
        }

        // Don't allow movement if over weight limit
        // ignore NPC that are over weight they can always move
        if (
            player.ConnectionId != "mob"
            && player.Weight > player.Attributes.Attribute[EffectLocation.Strength] * 3
        )
        {
            Services.Instance.Writer.WriteLine(
                $"<p>You are over encumbered and cannot move.</p>",
                player
            );
            return;
        }

        Exit getExitToNextRoom = null;

        switch (input[0])
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

        if (getExitToNextRoom == null)
        {
            Services.Instance.Writer.WriteLine("<p>You can't go that way.</p>", player);
            return;
        }

        var nextRoomKey =
            $"{getExitToNextRoom.AreaId}{getExitToNextRoom.Coords.X}{getExitToNextRoom.Coords.Y}{getExitToNextRoom.Coords.Z}";
        var getNextRoom = Services.Instance.Cache.GetRoom(nextRoomKey);

        if (getNextRoom == null)
        {
            Services.Instance.Writer.WriteLine(
                "<p>A mysterious force prevents you from going that way.</p>",
                player
            );
            //TODO: log bug that the new room could not be found
            return;
        }

        if (getExitToNextRoom.Closed)
        {
            Services.Instance.Writer.WriteLine("<p>The door is close.</p>", player);
            return;
        }

        if (string.IsNullOrEmpty(player.Mounted.Name))
        {
            player.Attributes.Attribute[EffectLocation.Moves] -= 1;
        }

        player.ChangeRoom(room, getNextRoom);

        if (player.Followers.Count >= 1)
        {
            foreach (
                var follower in player.Followers.Where(
                    follower => room.Players.Contains(follower) || room.Mobs.Contains(follower)
                )
            )
            {
                follower.ChangeRoom(room, getNextRoom);
            }
        }

        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            var mountedMob = room.Mobs.FirstOrDefault(
                x =>
                    !string.IsNullOrEmpty(x.Mounted.MountedBy)
                    && x.Mounted.MountedBy.Equals(player.Name)
            );

            if (mountedMob != null)
            {
                mountedMob.ChangeRoom(room, getNextRoom);
            }
        }
    }

    private bool CharacterCanMove(Player character)
    {
        return character.ConnectionId == "mob"
            || character.Attributes.Attribute[EffectLocation.Moves] > 0;
    }
}
