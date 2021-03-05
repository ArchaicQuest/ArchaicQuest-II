using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Crafting;
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

        bool AddRoom(string id, Room room);
        bool AddOriginalRoom(string id, Room room);
        List<Room> GetAllRoomsToRepop();
        List<Room> GetOriginalRooms();
        List<Room> GetAllRoomsInArea(int id);
        List<Room> GetAllRooms();
        Room GetRoom(string id);
        Room GetOriginalRoom(string id);
        bool UpdateRoom(string id, Room room, Player player);


        bool AddSkill(int id, Skill.Model.Skill skill);

        Skill.Model.Skill GetSkill(int id);
        void ClearRoomCache();
        void SetConfig(Config config);
        Config GetConfig();

        /// <summary>
        /// areaId + Zindex
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="room"></param>
        void AddMap(string areaId, string room);
        /// <summary>
        /// Area Id + Z coord
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        string GetMap(string areaId);

        bool IsCharInCombat(string id);
        bool AddCharToCombat(string id, Player character);
        Player GetCharFromCombat(string id);
        Player RemoveCharFromCombat(string id);
        List<Player> GetCombatList();

        /// <summary>
        /// Experiment, Need to generate the commands list on start up
        /// So things like socials & skills can work dynamically without
        /// needing to change the backend code
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Action> GetCommands();

        void AddCommand(string key, Action action);

        public void AddSocial(string key, Emote emote);
        public Dictionary<string, Emote> GetSocials();

        bool AddQuest(int id, Quest quest);
        Quest GetQuest(int id);
        ConcurrentDictionary<int, Quest> GetQuestCache();

        public bool AddHelp(int id, Help help);
        public Help GetHelp(int id);
        public List<Help> FindHelp(string id);

        public bool AddCraftingRecipes(int id, CraftingRecipes CraftingRecipes);
        public CraftingRecipes GetCraftingRecipes(int id, CraftingRecipes recipe);
        public List<CraftingRecipes> GetCraftingRecipes();

        public bool AddClass(string id, Class pcClass);

        public Class GetClass(string id);
    }
}