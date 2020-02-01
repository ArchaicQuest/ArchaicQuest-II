using System;
using System.Collections.Concurrent;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ICache
    {     
        /// <summary>
        /// Add player to cache
        /// </summary>
        /// <returns>returns player Cache</returns>
        bool AddPlayer(string id, Player player);

        bool PlayerAlreadyExists(Guid id);
    }
}