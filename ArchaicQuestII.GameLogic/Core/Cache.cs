using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Emote;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    /// <summary>
    /// Refactor me
    /// TODO: refactor cache
    /// </summary>
    public class Cache : ICache
    {
        private readonly ConcurrentDictionary<string, Player> _playerCache = new();
        private readonly ConcurrentDictionary<string, Room> _originalRoomCache = new();
        private readonly ConcurrentDictionary<string, Room> _roomCache = new();
        private readonly ConcurrentDictionary<int, Skill.Model.Skill> _skillCache = new();
        private readonly ConcurrentDictionary<string, string> _mapCache = new();
        private readonly ConcurrentDictionary<string, Player> _combatCache = new();
        private readonly ConcurrentDictionary<int, Quest> _questCache = new();
        private readonly ConcurrentDictionary<int, Help> _helpCache = new();
        private readonly ConcurrentDictionary<int, CraftingRecipes> _craftingRecipesCache = new();
        private readonly Dictionary<string, Emote> _socials = new();
        private readonly Dictionary<string, ICommand> _commands = new();
        private Config _configCache = new();

        #region Commands

        public void AddCommand(string key, ICommand action)
        {
            _commands.Add(key, action);
        }

        public Dictionary<string, ICommand> GetCommands()
        {
            return _commands;
        }

        public bool IsCommand(string key)
        {
            return _commands.TryGetValue(key, out var c);
        }

        public ICommand GetCommand(string key)
        {
            _commands.TryGetValue(key, out var command);
            return command;
        }

        #endregion

        #region PlayerCache

        public bool AddPlayer(string id, Player player)
        {
            return _playerCache.TryAdd(id, player);
        }

        public Player GetPlayer(string id)
        {
            _playerCache.TryGetValue(id, out Player player);

            return player;
        }

        public List<Player> GetAllPlayers()
        {
            return _playerCache.Values.ToList();
        }

        public Player RemovePlayer(string id)
        {
            _playerCache.TryRemove(id, out Player player);
            return player;
        }

        /// <summary>
        /// Only for the main loop
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, Player> GetPlayerCache()
        {
            return _playerCache;
        }

        public Player PlayerAlreadyExists(Guid id)
        {
            return _playerCache.Values.FirstOrDefault(x => x.Id.Equals(id));
        }

        #endregion

        #region RoomCache


        public bool AddRoom(string id, Room room)
        {
            return _roomCache.TryAdd(id, room);
        }

        public bool AddOriginalRoom(string id, Room room)
        {
            return _originalRoomCache.TryAdd(id, room);
        }

        //public Room GetRoom(int id)
        //{
        //    _roomCache.TryGetValue(id == 0 ? 1 : id, out Room room);

        //    return room;
        //}
        /// <summary>
        /// This is not as efficient as get by id
        /// Won't be an issue for a long time so can be revisted.
        /// to fix, need a field in the admin exit modal for roomId
        /// of the room you want to move to from your current position
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Room GetRoom(string id)
        {
            _roomCache.TryGetValue(id, out var room);

            return room;
        }

        public Room GetOriginalRoom(string id)
        {
            _originalRoomCache.TryGetValue(id, out var room);

            return room;
        }

        public List<Room> GetAllRooms()
        {
            var room = _roomCache.Values.ToList();

            return room;
        }

        public List<Room> GetOriginalRooms()
        {
            return _originalRoomCache.Values.ToList();
        }

        public List<Room> GetAllRoomsInArea(int id)
        {
            var room = _roomCache.Values.Where(x => x.AreaId.Equals(id)).ToList();

            return room;
        }

        public List<Room> GetAllRoomsToRepop()
        {
            var room = _roomCache.Values.ToList();

            return room;
        }

        public List<Skill.Model.Skill> GetAllSkills()
        {
            var data = _skillCache.Values.ToList();

            return data;
        }

        public bool UpdateRoom(string id, Room room, Player player)
        {
            var existingRoom = room;
            var newRoom = room;
            newRoom.Players.Add(player);

            return _roomCache.TryUpdate(id, existingRoom, newRoom);
        }

        #endregion

        #region SkillCache


        public bool AddSkill(int id, Skill.Model.Skill skill)
        {
            return _skillCache.TryAdd(id, skill);
        }

        public Skill.Model.Skill GetSkill(int id)
        {
            _skillCache.TryGetValue(id == 0 ? 1 : id, out var skill);

            return skill;
        }

        public List<Skill.Model.Skill> ReturnSkills()
        {
            return _skillCache.Values.ToList();
        }

        #endregion

        #region HelpCache


        public bool AddHelp(int id, Help help)
        {
            return _helpCache.TryAdd(id, help);
        }

        public Help GetHelp(int id)
        {
            _helpCache.TryGetValue(id, out var help);

            return help;
        }

        public List<Help> FindHelp(string id)
        {
            return _helpCache.Values
                .Where(
                    x =>
                        x.Keywords.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                        && x.Deleted.Equals(false)
                )
                .ToList();
        }

        #endregion

        #region Recipes

        public List<CraftingRecipes> GetCraftingRecipes()
        {
            return _craftingRecipesCache.Values.ToList();
        }

        public bool AddCraftingRecipes(int id, CraftingRecipes CraftingRecipes)
        {
            return _craftingRecipesCache.TryAdd(id, CraftingRecipes);
        }

        public CraftingRecipes GetCraftingRecipes(int id, CraftingRecipes recipe)
        {
            _craftingRecipesCache.TryGetValue(id, out var data);

            return data;
        }
        #endregion

        public void SetConfig(Config config)
        {
            _configCache = config;
        }

        public Config GetConfig()
        {
            return _configCache;
        }

        public void ClearRoomCache()
        {
            _roomCache.Clear();
            _originalRoomCache.Clear();
            _mapCache.Clear();
            _helpCache.Clear();
            _questCache.Clear();
            _skillCache.Clear();
            _craftingRecipesCache.Clear();
        }

        public void AddMap(string areaId, string map)
        {
            _mapCache.TryAdd(areaId, map);
        }

        public string GetMap(string areaId)
        {
            _mapCache.TryGetValue(areaId, out var map);

            return map;
        }

        #region mobs or players fighting

        public bool IsCharInCombat(string id)
        {
            return _combatCache.ContainsKey(id);
        }

        public bool AddCharToCombat(string id, Player character)
        {
            return _combatCache.TryAdd(id, character);
        }

        public Player GetCharFromCombat(string id)
        {
            _combatCache.TryGetValue(id, out Player character);

            return character;
        }

        public Player RemoveCharFromCombat(string id)
        {
            _combatCache.TryRemove(id, out Player character);
            return character;
        }

        public List<Player> GetCombatList()
        {
            return _combatCache.Values.ToList();
        }

        public void AddSocial(string key, Emote emote)
        {
            _socials.Add(key, emote);
        }

        public Dictionary<string, Emote> GetSocials()
        {
            return _socials;
        }

        public bool AddQuest(int id, Quest quest)
        {
            return _questCache.TryAdd(id, quest);
        }

        public Quest GetQuest(int id)
        {
            _questCache.TryGetValue(id, out var quest);

            return quest;
        }

        public ConcurrentDictionary<int, Quest> GetQuestCache()
        {
            return _questCache;
        }

        #endregion
    }
}
