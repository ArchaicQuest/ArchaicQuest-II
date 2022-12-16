using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Account;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Immortal;

public class ImmTeleportCmd : ICommand
{
    public ImmTeleportCmd(ICore core)
    {
        Aliases = new[] {"immteleport"};
        Description = "Immortal teleport";
        Usages = new[] {"Type: teleport 1010"};
        DeniedStatus = default;
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
            Core.Writer.WriteLine("<p>Teleport to what room, or to whom?</p>");
            return;
        }

        if (int.TryParse(target, out var roomId))
        {
            var newRoom = Core.Cache.GetRoom(roomId.ToString());

            if (newRoom != null)
            {
                Core.RoomActions.RoomChange(player, room, newRoom);
            }
            else
            {
                Core.Writer.WriteLine("<p>That room does not exist.</p>", player.ConnectionId);
            }
        }
        else
        {
            Player foundPlayer = null;
            
            foreach (var checkRoom in Core.Cache.GetAllRooms().TakeWhile(checkRoom => foundPlayer == null))
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
                Core.Writer.WriteLine("<p>They're not here.</p>", player.ConnectionId);
                return;
            }
            
            Core.RoomActions.RoomChange(player, room, Core.Cache.GetRoom(foundPlayer.RoomId));
        }
    }
}