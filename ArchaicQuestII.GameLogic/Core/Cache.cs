using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Cache : ICache
    {
        private readonly ConcurrentDictionary<string, Player> _playerCache = new ConcurrentDictionary<string, Player>();
        private readonly ConcurrentDictionary<int, Room> _roomCache = new ConcurrentDictionary<int, Room>();

        public bool AddPlayer(string id, Player player)
        {
            return _playerCache.TryAdd(id, player);
        }

        public bool PlayerAlreadyExists(Guid id)
        {
            return _playerCache.Values.Any(x => x.Id.Equals(id));
        }

        public bool AddRoom(int id, Room room)
        {
            return _roomCache.TryAdd(id, room);
        }

        public Room GetRoom(int id)
        {
            _roomCache.TryGetValue(id, out Room room);

            return room;
        }

        public bool UpdateRoom(int id, Room room, Player player)
        {
            var existingRoom = room;
            var newRoom = room;
            newRoom.Players.Add(player);


            return _roomCache.TryUpdate(id, existingRoom, newRoom);
        }

    }
}
