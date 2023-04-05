using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class RoomEmoteLoop : ILoop
    {
        public int TickDelay => 45000;
        public bool ConfigureAwait => false;
        private ICore _core;
        private List<Room> _rooms;

        public void Init(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
        }

        public void PreTick()
        {
            _rooms = _core.Cache.GetAllRooms().Where(x => x.Players.Any() && x.Emotes.Any()).ToList();
        }

        public void Tick()
        {
            foreach (var room in _rooms)
            {
                if (DiceBag.Roll(1, 1, 10) < 7)
                {
                    continue;
                }

                var emote = room.Emotes[DiceBag.Roll(1, 0, room.Emotes.Count - 1)];

                foreach (var player in room.Players)
                {
                    _core.Writer.WriteLine($"<p class='room-emote'>{emote}</p>",
                        player.ConnectionId);
                }
            }
        }

        public void PostTick()
        {
            _rooms.Clear();
        }
    }
}

