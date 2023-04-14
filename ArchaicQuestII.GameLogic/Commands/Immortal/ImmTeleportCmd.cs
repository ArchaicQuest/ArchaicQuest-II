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
            Services.Instance.Writer.WriteLine("<p>Teleport to what room, or to whom?</p>", player);
            return;
        }

        if (int.TryParse(target, out var roomId))
        {
            var newRoom = Services.Instance.Cache.GetRoom(roomId.ToString());

            if (newRoom != null)
            {
                player.ChangeRoom(room, newRoom);
            }
            else
            {
                Services.Instance.Writer.WriteLine("<p>That room does not exist.</p>", player);
            }
        }
        else
        {
            Player foundPlayer = null;

            foreach (
                var checkRoom in Services.Instance.Cache
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
                Services.Instance.Writer.WriteLine("<p>They're not here.</p>", player);
                return;
            }

            player.ChangeRoom(room, Services.Instance.Cache.GetRoom(foundPlayer.RoomId));
        }
    }
}
