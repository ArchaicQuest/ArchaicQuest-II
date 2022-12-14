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
    public MoveCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
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
            "northeast",
            "ne",
            "northwest",
            "nw",
            "up",
            "u",
            "down",
            "d"
        };
        Description = "Tries to move your character to the direction typed.";
        Usages = new[] { "Type: north", "Type: southwest" };
        UserRole = UserRole.Player;
        Writer = writeToClient;
        Cache = cache;
        UpdateClient = updateClient;
        RoomActions = roomActions;
    }
    
    public string[] Aliases { get; }
    public string Description { get; }
    public string[] Usages { get; }
    public UserRole UserRole { get; }
    public IWriteToClient Writer { get; }
    public ICache Cache { get; }
    public IUpdateClientUI UpdateClient { get; }
    public IRoomActions RoomActions { get; }

    public void Execute(Player player, Room room, string[] input)
    {
        if (CharacterCanMove(player) == false)
        {
            Writer.WriteLine("<p>You are too exhausted to move.</p>", player.ConnectionId);
            return;
        }
        
        switch (player.Status)
        {
            case CharacterStatus.Status.Fighting:
            case CharacterStatus.Status.Incapacitated:
                Writer.WriteLine("<p>NO WAY! you are fighting.</p>", player.ConnectionId);
                return;
            case CharacterStatus.Status.Resting:
                Writer.WriteLine("<p>Nah... You feel too relaxed to do that.</p>", player.ConnectionId);
                return;
            case CharacterStatus.Status.Sitting:
                Writer.WriteLine("<p>You can't do that while sitting.</p>", player.ConnectionId);
                return;
            case CharacterStatus.Status.Sleeping:
                Writer.WriteLine("<p>In your dreams.</p>", player.ConnectionId);
                return;
            case CharacterStatus.Status.Stunned:
                Writer.WriteLine("<p>You are too stunned to move.</p>", player.ConnectionId);
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
            Writer.WriteLine("<p>You can't go that way.</p>", player.ConnectionId);
            return;
        }

        var nextRoomKey =
            $"{getExitToNextRoom.AreaId}{getExitToNextRoom.Coords.X}{getExitToNextRoom.Coords.Y}{getExitToNextRoom.Coords.Z}";
        var getNextRoom = Cache.GetRoom(nextRoomKey);

        if (getNextRoom == null)
        {
            Writer.WriteLine("<p>A mysterious force prevents you from going that way.</p>", player.ConnectionId);
            //TODO: log bug that the new room could not be found
            return;
        }

        if (getExitToNextRoom.Closed)
        {
            Writer.WriteLine("<p>The door is close.</p>", player.ConnectionId);
            return;
        }

        if (string.IsNullOrEmpty(player.Mounted.Name))
        {
            player.Attributes.Attribute[EffectLocation.Moves] -= 1;

            if (player.Attributes.Attribute[EffectLocation.Moves] < 0)
            {
                player.Attributes.Attribute[EffectLocation.Moves] = 0;
            }
        }

        RoomActions.RoomChange(player, room, getNextRoom);

        if (player.Followers.Count >= 1)
        {
            foreach (var follower in player.Followers)
            {
                if (room.Players.Contains(follower) || room.Mobs.Contains(follower))
                {
                    RoomActions.RoomChange(follower, room, getNextRoom);
                }
            }
        }

        if (!string.IsNullOrEmpty(player.Mounted.Name))
        {
            var mountedMob = room.Mobs.FirstOrDefault(x => 
                !string.IsNullOrEmpty(x.Mounted.MountedBy) && x.Mounted.MountedBy.Equals(player.Name));

            if (mountedMob != null)
            {
                RoomActions.RoomChange(mountedMob, room, getNextRoom);
            }
        }
    }
    
    private bool CharacterCanMove(Player character)
    {
        return character.ConnectionId == "mob" || character.Attributes.Attribute[EffectLocation.Moves] > 0;
    }
}