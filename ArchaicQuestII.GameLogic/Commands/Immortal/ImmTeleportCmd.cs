using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmTeleportCmd : ICommand
{
    public ImmTeleportCmd(IWriteToClient writeToClient, ICache cache, IUpdateClientUI updateClient, IRoomActions roomActions)
    {
        Aliases = new[] {"immteleport"};
        Description = "Immortal teleport";
        Usages = new[] {"Type: teleport 1010"};
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
        var target = input.ElementAtOrDefault(1);

        if (string.IsNullOrEmpty(target))
        {
            Writer.WriteLine("<p>Teleport to what room, or to whom?</p>");
            return;
        }

        if (int.TryParse(target, out var roomId))
        {
            var newRoom = Cache.GetRoom(roomId.ToString());

            if (newRoom != null)
            {
                RoomActions.RoomChange(player, room, newRoom);
            }
            else
            {
                Writer.WriteLine("<p>That room does not exist.</p>", player.ConnectionId);
            }
        }
        else
        {
            Player foundPlayer = null;
            
            foreach (var checkRoom in Cache.GetAllRooms().TakeWhile(checkRoom => foundPlayer == null))
            {
                foreach (var checkRoomPlayer in checkRoom.Players
                             .TakeWhile(checkRoomPlayer => foundPlayer == null)
                             .Where(checkRoomPlayer => checkRoomPlayer.Name
                                 .StartsWith(target, StringComparison.CurrentCultureIgnoreCase)))
                {
                    foundPlayer = checkRoomPlayer;
                }
            }

            if (foundPlayer == null)
            {
                Writer.WriteLine("<p>They're not here.</p>", player.ConnectionId);
                return;
            }
            
            RoomActions.RoomChange(player, room, Cache.GetRoom(foundPlayer.RoomId));
        }
    }
}