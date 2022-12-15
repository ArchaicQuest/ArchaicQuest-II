using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ICache
    {
        void SetDatabase(IDataBase db);
        IDataBase GetDatabase();
        void SetPlayerDatabase(IPlayerDataBase pdb);
        IPlayerDataBase GetPlayerDatabase();
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
        List<Skill.Model.Skill> GetAllSkills();
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
        List<Skill.Model.Skill> ReturnSkills();

        /// <summary>
        /// Experiment, Need to generate the commands list on start up
        /// So things like socials & skills can work dynamically without
        /// needing to change the backend code
        /// </summary>
        /// <returns></returns>
        Dictionary<string, ICommand> GetCommands();

        bool GetCommand(string key, out ICommand command);

        void AddCommand(string key, ICommand action);

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