using System.Linq;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.World.Loops;

public class UpdateRoomEmote : IGameLoop
{
    public int TickDelay => 45000;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }
    public void Loop()
    {
        var rooms = Handler.World.GetAllRooms().Where(x => x.Players.Any()).ToList();

        if (!rooms.Any())
        {
            return;
        }

        foreach (var room in rooms)
        {
            if (!room.Emotes.Any() || DiceBag.Roll(1, 1, 10) < 7)
            {
                continue;
            }

            var emote = room.Emotes[DiceBag.Roll(1, 0, room.Emotes.Count - 1)];

            foreach (var player in room.Players)
            {
                Handler.Client.WriteLine($"<p class='room-emote'>{emote}</p>",
                    player.ConnectionId);
            }
        }
    }
}