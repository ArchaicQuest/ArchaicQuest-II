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
    public ImmTeleportCmd()
    {
        Aliases = new[] { "immteleport" };
        Description = "Immortal teleport";
        Usages = new[] { "Type: immteleport 1010" };
        Title = "";
        DeniedStatus = null;
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
            CoreHandler.Instance.Writer.WriteLine("<p>Teleport to what room, or to whom?</p>");
            return;
        }

        if (int.TryParse(target, out var roomId))
        {
            var newRoom = CoreHandler.Instance.Cache.GetRoom(roomId.ToString());

            if (newRoom != null)
            {
                CoreHandler.Instance.RoomActions.RoomChange(player, room, newRoom, false);
            }
            else
            {
                CoreHandler.Instance.Writer.WriteLine(
                    "<p>That room does not exist.</p>",
                    player.ConnectionId
                );
            }
        }
        else
        {
            Player foundPlayer = null;

            foreach (
                var checkRoom in CoreHandler.Instance.Cache
                    .GetAllRooms()
                    .TakeWhile(checkRoom => foundPlayer == null)
            )
            {
                foreach (
                    var checkRoomPlayer in checkRoom.Players
                        .TakeWhile(checkRoomPlayer => foundPlayer == null)
                        .Where(
                            checkRoomPlayer =>
                                checkRoomPlayer.Name.StartsWith(
                                    target,
                                    StringComparison.CurrentCultureIgnoreCase
                                )
                        )
                )
                {
                    foundPlayer = checkRoomPlayer;
                }
            }

            if (foundPlayer == null)
            {
                CoreHandler.Instance.Writer.WriteLine(
                    "<p>They're not here.</p>",
                    player.ConnectionId
                );
                return;
            }

            CoreHandler.Instance.RoomActions.RoomChange(
                player,
                room,
                CoreHandler.Instance.Cache.GetRoom(foundPlayer.RoomId),
                false
            );
        }
    }
}
