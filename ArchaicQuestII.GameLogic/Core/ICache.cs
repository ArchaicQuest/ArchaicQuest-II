using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ICache
    {
        /// <summary>
        /// Add player to cache
        /// </summary>
        /// <returns>returns player Cache</returns>
        bool AddPlayer(string id, Player player);
        Player GetPlayer(string id);
        Player RemovePlayer(string id);
        ConcurrentDictionary<string, Player> GetPlayerCache();
        Player PlayerAlreadyExists(Guid id);

        bool AddRoom(int id, Room room);

        Room GetRoom(int id);
        List<Room> GetAllRoomsToRepop();
        List<Room> GetAllRoomsInArea(int id);
        Room GetRoom(int id, Coordinates coords);
        bool UpdateRoom(int id, Room room, Player player);


        bool AddSkill(int id, Skill.Model.Skill skill);

        Skill.Model.Skill GetSkill(int id);
        void ClearRoomCache();
        void SetConfig(Config config);
        Config GetConfig();

        void AddMap(int areaId, string room);
        string GetMap(int areaId);

        bool IsCharInCombat(string id);
        bool AddCharToCombat(string id, Player character);
        Player GetCharFromCombat(string id);
        Player RemoveCharFromCombat(string id);
        List<Player> GetCombatList();
    }
}