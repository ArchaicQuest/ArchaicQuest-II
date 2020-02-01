using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Cache : ICache
    {
        private readonly ConcurrentDictionary<string, Player> _playerCache = new ConcurrentDictionary<string, Player>();

        public bool AddPlayer(string id, Player player)
        {
            return _playerCache.TryAdd(id, player);
        }

        public bool PlayerAlreadyExists(Guid id)
        {
            return _playerCache.Values.Any(x => x.Id.Equals(id));
        }

    }
}
